using System;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Utility
{
    public class Holidays : IHolidays
    {
        private readonly int year;
        private readonly DateTime[] constHolidays;

        public Holidays(int year)
        {
            if (year < DateTime.MinValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Value must be greater than 0");
            }

            this.year = year;
            constHolidays = new[]
            {
                new DateTime(year, 1, 1),
                new DateTime(year, 1, 6),
                new DateTime(year, 5, 1),
                new DateTime(year, 5, 3),
                new DateTime(year, 8, 15),
                new DateTime(year, 11, 1),
                new DateTime(year, 11, 11),
                new DateTime(year, 12, 25),
                new DateTime(year, 12, 26)
            };
        }

        public IEnumerable<DateTime> HolidayDates
        {
            get
            {
                var easter = CalculateEasterDate();
                var holidays = new List<DateTime>
                {
                    easter
                };
                holidays.AddRange(CalculateEasterDependentThings(easter));
                holidays.AddRange(constHolidays);

                return holidays;
            }
        }

        public DateTime CalculateEasterDate()
        {
            int month = 3;

            int g = year % 19;
            int c = year / 100;
            int h = (c - c / 4 - (8 * c + 13) / 25 + 19 * g + 15) % 30;
            int i = h - h / 28 * (1 - h / 28 * (29 / (h + 1)) * ((21 - g) / 11));

            int day = i - (year + year / 4 + i + 2 - c + c / 4) % 7 + 28;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        private static IEnumerable<DateTime> CalculateEasterDependentThings(DateTime easter)
        {
            var collection = new List<DateTime>
            {
                easter.AddDays(1), // Poniedzialek Wielkanocny
                easter.AddDays(49), // Zielone Swiatki (1. dzien)
                easter.AddDays(60) // Boze Cialo
            };
            return collection;
        }
    }
}
