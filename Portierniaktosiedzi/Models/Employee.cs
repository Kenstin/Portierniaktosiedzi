﻿using System;
using System.Linq;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Employee
    {
        public Employee(decimal posts, Gender gender, string name)
        {
            Posts = posts;
            Gender = gender;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        public decimal Posts { get; set; }

        public virtual decimal GetWorkingHours(int month, int year, IHolidays holidays)
        {
            int workingdays = DateTime.DaysInMonth(year, month);

            workingdays -= new DateTime(year, month, 1).GetSaturdaysAndSundays().Count();

            foreach (var element in holidays.HolidayDates)
                {
                    if (element.Month == month && element.DayOfWeek != DayOfWeek.Saturday && element.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workingdays--;
                    }
                }

            return Posts * workingdays * 8;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
