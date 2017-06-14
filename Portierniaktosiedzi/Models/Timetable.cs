using System;
using System.Collections.Generic;
using System.Linq;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Timetable
    {
        public Timetable(DateTime month, IEnumerable<Day> daysBefore)
        {
            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }

            var days = daysBefore.ToList();

            Month = month;

            try
            {
                if (days.Any(day => !day.IsFull()))
                {
                    throw new ArgumentException("All shifts have to be assigned.");
                }
            }
            catch (ArgumentNullException exception)
            {
                throw new ArgumentNullException("No element can be null.", exception);
            }

            if (days.Count < 6)
            {
                throw new ArgumentOutOfRangeException(nameof(daysBefore), "Length has to be greater than 5");
            }

            Days = new NegativeArray<Day>(days, new Day[DateTime.DaysInMonth(month.Year, month.Month)]);

            for (int i = 1; i <= Days.RightArrayLength; i++)
            {
                Days[i] = new Day();
            }
        }

        public NegativeArray<Day> Days { get; }

        public DateTime Month { get; }

        public IReadOnlyDictionary<Employee, decimal> WorkingHoursLeft { get; protected set; }

        //11 h - 2 zmiany odstepu
        //35h w tygodniu - 5 odstepu
        //wolny weekend raz na miesiac
        //5 dni w tygodniu max
        //5 dni pod rzad max

        public void Generate(IEnumerable<Employee> employees, IHolidays holidays)
        {
            var timetableGenerator = new TimetableGenerator(this, employees, holidays ?? throw new ArgumentNullException(nameof(holidays)));
            timetableGenerator.Generate();
            WorkingHoursLeft = timetableGenerator.WorkingHoursLeft;
        }
    }
}
