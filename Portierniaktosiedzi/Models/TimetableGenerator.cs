using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class TimetableGenerator
    {
        private readonly Timetable timetable;
        private readonly List<Employee> employees;
        private readonly IHolidays holidays;
        private readonly List<Tuple<DateTime, DateTime>> weekends;
        private readonly Employee schoolEmployee;
        private Dictionary<Employee, int> workingHoursLeft;
        private Dictionary<Employee, Tuple<DateTime, DateTime>> weekendsOff;
        private Tuple<Dictionary<Employee, Tuple<DateTime, DateTime>>, int> bestCombination;

        public TimetableGenerator(Timetable timetable, IEnumerable<Employee> employees, IHolidays holidays)
        {
            this.timetable = timetable;
            this.employees = employees.OrderByDescending(e => e.Posts).ToList();
            this.holidays = holidays;

            weekendsOff = new Dictionary<Employee, Tuple<DateTime, DateTime>>();
           // weekends = new List<Tuple<DateTime, DateTime>>();
            weekends = timetable.Month.GetWeekendTuples().ToList();
            schoolEmployee = new Employee(float.MaxValue, Gender.Woman, "Pracownik szkoly");
            bestCombination = Tuple.Create(weekendsOff, int.MaxValue);
        }

        public bool Generate()
        {
            if (BruteForce())
            {
                return true;
            }

            weekendsOff = bestCombination.Item1;
            TryMake();
            return true;
        }

        private void AddToShift(Employee employee, int day, int shift)
        {
            if (employee != null)
            {
                timetable.Days[day].Shifts[shift] = employee;
                workingHoursLeft[employee] -= 8;
            }
        }

        private bool BruteForce()
        {
            var nextEmployee = employees.Find(e => !weekendsOff.ContainsKey(e));
            if (nextEmployee == null) // jesli nie znajdziemy juz employeea bez przydzielonego wolnego weekendu,
            {
                //  to znaczy, ¿e mo¿na generowaæ grafik
                return TryMake();
            }

            // w przeciwnym wypadku (jesli byl employee bez wolnego weekendu)
            foreach (var weekend in weekends)
            {
                weekendsOff.Add(nextEmployee, weekend);
                // employee[e].week = weekends[i] // <= to employeeowi[e] przypisujemy weekend[i]
                if (BruteForce())
                {
                    return true;
                }// i wywolujac rekurencje robimy to samo dla kolejnych

                weekendsOff.Remove(nextEmployee); //jesli XD() zwrocilo nam false to znaczy, ze nie udalo sie wygenerowac poprawnego grafiku i cofamy przypisanie weekendu
            }

            return false; //zwracamy false bo kombinacja nie byla prawidlowa
        }

        private bool TryMake()
        {
            /*for (var i = 0; i < employees.Count; i++)
            {
                weekendsOff[employees[i]] = weekends[i % weekends.Count];
            }*/

            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                timetable.Days[i] = new Day();
            }

            workingHoursLeft = employees.ToDictionary(
                employee => employee, employee => employee.GetWorkingHours(timetable.Month.Month, timetable.Month.Year, holidays));

            var boys = employees.Where(e => e.Gender == Gender.Man).ToList();

            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                var first = boys.FirstOrDefault(e => MeetsCriteria(e, i, 2));
                AddToShift(first, i, 2);
            }

            /*foreach (var day in days)
            {
                var first = girls.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
                workingHoursLeft[first] -= 8;
                day.Shifts[1] = first;
            }*/

           // var employeesLeft = employees.Where(e => workingHoursLeft[e] > 0).ToList();

            var daysOff = new List<DateTime>();
            daysOff.AddRange(holidays.HolidayDates.Where(date => date.Month == timetable.Month.Month));
            daysOff.AddRange(timetable.Month.GetSaturdaysAndSundays());
            daysOff = daysOff.Distinct().ToList();

            foreach (var date in daysOff)
            {
                var emp = employees.FirstOrDefault(e => MeetsCriteria(e, date.Day, 1));
                AddToShift(emp, date.Day, 1);

                emp = employees.FirstOrDefault(e => MeetsCriteria(e, date.Day, 0));
                AddToShift(emp, date.Day, 0);
            }

            /*for (var i = 0; i < days.Length; i++)
            {
                var day = days[i];
                for (var j = 0; j < day.Shifts.Length; j++)
                {
                    if (day.Shifts[j] == null)
                    {
                        var emp = employeesLeft.First(e => workingHoursLeft[e] > 0 &&
                                                           MeetsCriteria(e, workingHoursLeft, i, j));
                        day.Shifts[j] = emp;
                        workingHoursLeft[emp] -= 8;
                    }
                }
            }*/

            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                if (timetable.Days[i].Shifts[1] == null)
                {
                    var emp = employees.FirstOrDefault(e => MeetsCriteria(e, i, 1));
                    AddToShift(emp, i, 1);
                }
            }

            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                if (timetable.Days[i].Shifts[0] == null)
                {
                    var emp = employees.FirstOrDefault(e => MeetsCriteria(e, i, 0));
                    if (emp != null)
                    {
                        AddToShift(emp, i, 0);
                    }
                }
            }

            var days = timetable.Days.Where(day => !day.IsFull());
            foreach (var day in days)
            {
                if (day.Shifts[0] == null)
                {
                    day.Shifts[0] = schoolEmployee;
                }
            }

            if (timetable.Days.Any(day => day.Shifts.Any(shift => shift == null)))
            {
                return false;
            }

            if (workingHoursLeft.All(pair => pair.Value == 0))
            {
                return true;
            }
            else
            {
                var i = workingHoursLeft.Sum(pair => pair.Value);
                if (i < bestCombination.Item2)
                {
                    bestCombination = Tuple.Create(new Dictionary<Employee, Tuple<DateTime, DateTime>>(weekendsOff), i);
                }
            }

            return false;
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:BracesMustNotBeOmitted", Justification = "Reviewed.")]
        private bool MeetsCriteria(Employee employee, int day, int shift)
        {
            var date = new DateTime(timetable.Month.Year, timetable.Month.Month, day);

            if (workingHoursLeft[employee] < 8)
                return false;

            if (day == weekendsOff[employee].Item1.Day || day == weekendsOff[employee].Item2.Day)
                return false;

            switch (shift) //checks two-shift break
            {
                case 2:
                    if (timetable.Days[day].Shifts[1] == employee || timetable.Days[day].Shifts[0] == employee || timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee || timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[1] == employee)
                        return false;
                    break;
                case 1:
                    if (timetable.Days[day].Shifts[0] == employee || timetable.Days[day - 1].Shifts[2] == employee || timetable.Days[day].Shifts[2] == employee || timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee)
                        return false;
                    break;
                case 0:
                    if (timetable.Days[day - 1].Shifts[2] == employee || timetable.Days[day - 1].Shifts[1] == employee || timetable.Days[day].Shifts[1] == employee || timetable.Days[day].Shifts[2] == employee)
                        return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shift));
            }

           /* if (day % 7 == 6) //&& shift == 1) //35h
            {
                int biggestGapSoFar = 0;
                int biggestGapNow = 0;
                for (int i = day - 5; i < day; i++)
                {
                    foreach (var j in timetable.Days[i].Shifts)
                    {
                        if (j != employee)
                        {
                            biggestGapNow++;
                            if (biggestGapNow > biggestGapSoFar)
                            {
                                biggestGapSoFar = biggestGapNow;
                            }
                        }
                        else
                        {
                            biggestGapNow = 0;
                        }
                    }
                }

                if (biggestGapNow > 0)
                {
                    biggestGapNow += shift;
                    if (biggestGapNow > biggestGapSoFar)
                    {
                        biggestGapSoFar = biggestGapNow;
                    }
                }

                if (biggestGapSoFar < 5)
                    return false;
            }
            */
            {
                //5 days per week
                int daysOff = 0;
                DateTime tmp = date.Clone();
                while (tmp.DayOfWeek != DayOfWeek.Monday)
                {
                    tmp = tmp.AddDays(-1);
                    if (tmp.Month == date.Month && !timetable.Days[tmp.Day].IsThere(employee))
                    {
                        daysOff++;
                    }
                    else if (tmp.Month == date.Month - 1 &&
                             timetable.Days[tmp.Day - DateTime.DaysInMonth(tmp.Year, tmp.Month)].IsThere(employee))
                    {
                        daysOff++;
                    }
                }

                tmp = date.Clone();
                while (tmp.DayOfWeek != DayOfWeek.Sunday)
                {
                    tmp = tmp.AddDays(1);
                    if (tmp.Month == date.Month && !timetable.Days[tmp.Day].IsThere(employee))
                    {
                        daysOff++;
                    }
                    else if (tmp.Month == date.Month - 1 &&
                             timetable.Days[tmp.Day - DateTime.DaysInMonth(tmp.Year, tmp.Month)].IsThere(employee))
                    {
                        daysOff++;
                    }
                }

                if (daysOff < 2)
                    return false;
        }

            /*if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) //5 days per week (mon-sun)
            {
                int daysOff = 0;

                DateTime tmp = date.AddDays(-1);
                do
                {
                    if (tmp.Month == date.Month && !timetable.Days[tmp.Day].IsThere(employee))
                    {
                        daysOff++;
                    }
                    else if (tmp.Month == date.Month - 1 && timetable.Days[tmp.Day - DateTime.DaysInMonth(tmp.Year, tmp.Month)].IsThere(employee))
                    {
                        daysOff++;
                    }

                    tmp = tmp.AddDays(-1);
                }
                while (tmp.DayOfWeek != DayOfWeek.Sunday); //na pewno xD?

                if (daysOff < 2)
                {
                    return false;
                }
            }*/

            int workingDays = 0;
            for (var i = day - 5; i < day; i++) //5 days in a row, to tez jest zjebane
            {
                if (timetable.Days[i].IsThere(employee))
                {
                    workingDays++;
                }
            }

            {
                DateTime tmp = date.Clone().AddDays(1);
                while (timetable.Days[tmp.Day].IsThere(employee))
                {
                    workingDays++;
                    tmp = tmp.AddDays(1);
                }
            }

            return workingDays < 5;
        }
    }
}