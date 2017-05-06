using System;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Employee
    {
        public Employee(float posts, Gender gender, string name)
        {
            Posts = posts;
            Gender = gender;
            Name = name;
        }

        public string Name { get; }

        public Gender Gender { get; }

        public float Posts { get; }

        public int GetWorkingHours(int month, int year, IHolidays holidays)
        {
            int workingdays = DateTime.DaysInMonth(year, month);

            foreach (var i in new DateTime(year, month, 1).GetSaturdaysAndSundays())
            {
                workingdays--;
            }

            foreach (var element in holidays.HolidayDates)
                {
                    if (element.Month == month && element.DayOfWeek != DayOfWeek.Saturday && element.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workingdays--;
                    }
                }

            int hours = (int)((Posts * (workingdays * 8)) - ((Posts * (workingdays * 8)) % 8));
            return hours;
        }
    }
}
