using System;
using System.Globalization;
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
            int hours, workingdays;
            int daysinmonth = System.DateTime.DaysInMonth(year, month);
            workingdays = System.DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= daysinmonth; i++)
            {
                DateTime dateValue = new DateTime(year, month, i);
                if (dateValue.ToString("dddd", CultureInfo.InvariantCulture) == "Saturday" || dateValue.ToString("dddd", CultureInfo.InvariantCulture) == "Sunday")
                {
                    workingdays--;
                }
            }

            foreach (var element in holidays.GetHolidays())
                {
                    if (element.Month == month)
                    {
                        workingdays--;
                    }
                }

            hours = (int)((Posts * (workingdays * 8)) - ((Posts * (workingdays * 8)) % 8));
            return hours;
        }
    }
}
