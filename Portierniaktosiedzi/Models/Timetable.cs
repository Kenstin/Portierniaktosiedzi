using System;
using System.Collections.Generic;
using System.Linq;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Timetable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Timetable"/> class.
        /// </summary>
        /// <param name="month">Month of the timetable</param>
        /// <param name="daysBefore">Six days before the 1st day of <see cref="month"/> needed to generate the timetable </param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="month"/> is null or when any day in daysBefore is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when daysBefore length is 5 or less</exception>
        /// <exception cref="ArgumentException">Thrown when not all shifts are assigned</exception>
        public Timetable(DateTime month, IEnumerable<Day> daysBefore)
        {
            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }
            
            Month = month.Clone();

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

        public bool Generate(IEnumerable<Employee> employees, IHolidays holidays)
        {
            var timetableGenerator = new TimetableGenerator(this, employees, holidays ?? throw new ArgumentNullException(nameof(holidays)));
            if (timetableGenerator.Generate())
            {
                WorkingHoursLeft = timetableGenerator.WorkingHoursLeft;
                return true;
            }

            return false;
        }
    }
}
