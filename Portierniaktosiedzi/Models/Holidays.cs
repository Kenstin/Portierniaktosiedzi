using System;

namespace Portierniaktosiedzi.Models
{
    public class Holidays
    {
        private readonly int year;

        public Holidays(int year)
        {
            if (year < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Value must be greater than 0");
            }

            this.year = year;
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
    }
}
