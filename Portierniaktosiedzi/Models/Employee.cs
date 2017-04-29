using System;
using System.Globalization;

namespace Portierniaktosiedzi.Models
{
    public class Employee
    {
        public Employee(int posts, Gender gender, string name)
        {
            Posts = posts;
            Gender = gender;
            Name = name;
        }

        public string Name { get; }

        public Gender Gender { get; }

        public int Posts { get; }

        public int PostsToWorkingHours(int month, int year)
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

            hours = (Posts * (workingdays * 8)) - ((Posts * (workingdays * 8)) % 8);
            return hours;
        }
    }
}
