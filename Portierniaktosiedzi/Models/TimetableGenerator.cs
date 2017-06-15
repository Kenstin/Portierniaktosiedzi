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
        private Dictionary<Employee, decimal> workingHoursLeft;
        private Dictionary<Employee, Tuple<DateTime, DateTime>> weekendsOff;
        private Tuple<Dictionary<Employee, Tuple<DateTime, DateTime>>, decimal> bestCombination;

        public TimetableGenerator(Timetable timetable, IEnumerable<Employee> employees, IHolidays holidays)
        {
            this.timetable = timetable;
            this.employees = employees.OrderByDescending(e => e.Posts).ToList();
            this.holidays = holidays;

            weekendsOff = new Dictionary<Employee, Tuple<DateTime, DateTime>>();
            weekends = timetable.Month.GetWeekendTuples().ToList();
            schoolEmployee = new Employee(float.MaxValue, Gender.Woman, "Pracownik szkoly");
            bestCombination = Tuple.Create(weekendsOff, decimal.MaxValue);
        }

        public IReadOnlyDictionary<Employee, decimal> WorkingHoursLeft => workingHoursLeft;

        public bool Generate()
        {
            if (BruteForce())
            {
                return true;
            }

            weekendsOff = bestCombination.Item1;
            if (weekendsOff.Count == 0)
            {
                return false;
            }

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
            if (nextEmployee == null) // jesli nie znajdziemy juz employeea bez przydzielonego wolnego weekendu, to generujemy
            {
                return TryMake();
            }

            // w przeciwnym wypadku (jesli byl employee bez wolnego weekendu)
            foreach (var weekend in weekends)
            {
                weekendsOff.Add(nextEmployee, weekend);
                if (BruteForce())
                {
                    return true;
                } // i wywolujac rekurencje robimy to samo dla kolejnych

                weekendsOff.Remove(nextEmployee); //jesli BruteForce() zwrocilo nam false to znaczy, ze nie udalo sie wygenerowac poprawnego grafiku i cofamy przypisanie weekendu
            }

            return false; //zwracamy false bo kombinacja nie byla prawidlowa
        }

        private bool TryMake()
        {
            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                timetable.Days[i] = new Day();
            }

            workingHoursLeft = employees.ToDictionary(
                employee => employee, employee => employee.GetWorkingHours(timetable.Month.Month, timetable.Month.Year, holidays));

            FillThirdShift();
            FillDaysOff();
            FillSecondShift();
            FillFirstShift();
            FillSchoolEmployees();

            if (timetable.Days.Any(day => day.Shifts.Any(shift => shift == null)))
            {
                return false;
            }

            if (workingHoursLeft.Any(pair => pair.Value != 0))
            {
                var i = workingHoursLeft.Sum(pair => pair.Value);
                if (i < bestCombination.Item2)
                {
                    bestCombination = Tuple.Create(new Dictionary<Employee, Tuple<DateTime, DateTime>>(weekendsOff), i);
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private void FillSchoolEmployees()
        {
            var days = timetable.Days.Where(day => !day.IsFull());
            foreach (var day in days)
            {
                if (day.Shifts[0] == null)
                {
                    day.Shifts[0] = schoolEmployee;
                }
            }
        }

        private void FillFirstShift()
        {
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
        }

        private void FillSecondShift()
        {
            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                if (timetable.Days[i].Shifts[1] == null)
                {
                    var emp = employees.FirstOrDefault(e => MeetsCriteria(e, i, 1));
                    AddToShift(emp, i, 1);
                }
            }
        }

        private void FillDaysOff()
        {
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
        }

        private void FillThirdShift()
        {
            var boys = employees.Where(e => e.Gender == Gender.Man).ToList();

            for (var i = 1; i <= timetable.Days.RightArrayLength; i++)
            {
                var first = boys.FirstOrDefault(e => MeetsCriteria(e, i, 2));
                AddToShift(first, i, 2);
            }
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:BracesMustNotBeOmitted", Justification = "Reviewed.")]
        private bool MeetsCriteria(Employee employee, int day, int shift)
        {
            var date = new DateTime(timetable.Month.Year, timetable.Month.Month, day);

            if (workingHoursLeft[employee] < 8)
                return false;

            if (day == weekendsOff[employee].Item1.Day || day == weekendsOff[employee].Item2.Day)
                return false;

            if (!CheckTwoShiftBreak(employee, day, shift))
                return false;

            if (!Check5DaysFromMonToSun(employee, date))
                return false;

            if (!Check5DaysInARow(employee, day, date))
                return false;

            return true;
        }

        private bool Check5DaysInARow(Employee employee, int day, DateTime date)
        {
            var workingDays = 0;
            for (var i = day - 5; i < day; i++)
            {
                if (timetable.Days[i].IsThere(employee))
                {
                    workingDays++;
                }
            }

            var tmp = date.Clone().AddDays(1);
            while (timetable.Days[tmp.Day].IsThere(employee))
            {
                workingDays++;
                tmp = tmp.AddDays(1);
            }

            return workingDays < 5;
        }

        private bool Check5DaysFromMonToSun(Employee employee, DateTime date)
        {
            var daysOff = 0;
            var tmp = date.Clone();
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
                else if (tmp.Month == date.Month + 1)
                {
                    daysOff++;
                }
            }

            return daysOff >= 2;
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:BracesMustNotBeOmitted", Justification = "Reviewed.")]
        private bool CheckTwoShiftBreak(Employee employee, int day, int shift)
        {
            switch (shift)
            {
                case 2:
                    if (timetable.Days[day].Shifts[1] == employee || timetable.Days[day].Shifts[0] == employee ||
                        timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee ||
                        timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[1] == employee)
                        return false;
                    break;
                case 1:
                    if (timetable.Days[day].Shifts[0] == employee || timetable.Days[day - 1].Shifts[2] == employee ||
                        timetable.Days[day].Shifts[2] == employee ||
                        timetable.Days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee)
                        return false;
                    break;
                case 0:
                    if (timetable.Days[day - 1].Shifts[2] == employee || timetable.Days[day - 1].Shifts[1] == employee ||
                        timetable.Days[day].Shifts[1] == employee || timetable.Days[day].Shifts[2] == employee)
                        return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shift));
            }

            return true;
        }
    }
}