using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Markup;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Timetable
    {
        private readonly Day[] days;
        private readonly List<Employee> employees;
        private readonly DateTime month;
        private readonly IHolidays holidays;
        //private Dictionary<Employee, ushort> xD;

        public Timetable(DateTime month, Day dayBefore, IEnumerable<Employee> employees, IHolidays holidays)
        {
            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }

            this.month = month;
            days = new Day[DateTime.DaysInMonth(month.Year, month.Month) + 1];
            days[0] = dayBefore ?? throw new ArgumentNullException(nameof(dayBefore));
            this.employees = employees.OrderBy(e => e.Posts).ToList();
            this.holidays = holidays ?? throw new ArgumentNullException(nameof(holidays));

            for (int i = 1; i < days.Length; i++)
            {
                days[i] = new Day();
            }

            /*xD = new Dictionary<Employee, ushort>
            {
                { this.employees[0], 0 },
                { this.employees[1], 0 },
                { this.employees[2], 0 }
            };*/
        }

        //11 h - 2 zmiany odstepu
        //35h w tygodniu - 5 odstepu
        //wolny weekend raz na miesiac
        //5 dni w tygodniu max
        //5 dni pod rzad max
        public void Generate()
        {
            var workingHoursLeft = employees.ToDictionary(
                employee => employee, employee => employee.GetWorkingHours(month.Month, month.Year, holidays));

            var boys = employees.Where(e => e.Gender == Gender.Man).ToList();

            for (var i = 1; i < days.Length; i++)
            {
                var day = days[i];
                var first = boys.First(e => MeetsCriteria(e, workingHoursLeft, i, 2));
                workingHoursLeft[first] -= 8;
                day.Shifts[2] = first;
            }

            /*foreach (var day in days)
            {
                var first = girls.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
                workingHoursLeft[first] -= 8;
                day.Shifts[1] = first;
            }*/

            var employeesLeft = employees.Where(e => workingHoursLeft[e] > 0).ToList();

            var daysOff = new List<DateTime>();
            daysOff.AddRange(holidays.HolidayDates.Where(date => date.Month == month.Month));
            daysOff.AddRange(month.GetSaturdaysAndSundays());
            foreach (var date in daysOff)
            {
                var emp = employeesLeft.First(e => MeetsCriteria(e, workingHoursLeft, date.Day, 1));
                days[date.Day].Shifts[1] = emp;
                workingHoursLeft[emp] -= 8;

                emp = employeesLeft.First(e => MeetsCriteria(e, workingHoursLeft, date.Day, 0));
                days[date.Day].Shifts[0] = emp;
                workingHoursLeft[emp] -= 8;
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
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:BracesMustNotBeOmitted", Justification = "Reviewed.")]
        private bool MeetsCriteria(Employee employee, Dictionary<Employee, int> workingHoursLeft, int day, int shift)
        {
            if (workingHoursLeft[employee] < 8)
                return false;

            /*if (day > 14 && xD.ContainsKey(employee) && xD[employee] < 2)
            {
                xD[employee]++;
                return false;
            }*/

            var date = new DateTime(month.Year, month.Month, day);

            switch (shift) //checks two-shift break
            {
                case 2:
                    if (days[day].Shifts[1] == employee || days[day].Shifts[0] == employee || days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee || days.ElementAtOrDefault(day + 1)?.Shifts[1] == employee)
                        return false;
                    break;
                case 1:
                    if (days[day].Shifts[0] == employee || days[day - 1].Shifts[2] == employee || days[day].Shifts[2] == employee || days.ElementAtOrDefault(day + 1)?.Shifts[0] == employee)
                        return false;
                    break;
                case 0:
                    if (days[day - 1].Shifts[2] == employee || days[day - 1].Shifts[1] == employee || days[day].Shifts[1] == employee || days[day].Shifts[2] == employee)
                        return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shift));
            }

            if (day % 7 == 6) //&& shift == 1) //35h
            {
                int biggestGapSoFar = 0;
                int biggestGapNow = 0;
                for (int i = day - 5; i < day; i++)
                {
                    foreach (var j in days[i].Shifts)
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

            if ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) && day >= 6)
            {
                int daysOff = 0;
                DateTime tmp = date.Clone().AddDays(-1);
                do
                {
                    if (!days[tmp.Day].IsThere(employee))
                    {
                        daysOff++;
                    }

                    tmp = tmp.AddDays(-1);
                }
                while (tmp.DayOfWeek != DayOfWeek.Monday);

                if (daysOff < 2)
                {
                    return false;
                }
            }

            int workingDays = 0;
            for (int i = day - 5; i < day; i++) //5 days in a row
            {
                if (i >= 0 && days[i].IsThere(employee))
                {
                    workingDays++;
                }
            }

            return workingDays < 5;
        }
    }
}
