using System;

namespace Portierniaktosiedzi.Models
{
    public class Month
    {
        private readonly Day[] days;
        private readonly Day[] daysBefore;

        private Month(Day[] days, Day[] daysBefore)
        {
            this.days = days;
            this.daysBefore = daysBefore;
        }

        /// <summary>
        /// Returns the Day, assuming that values greater than 0 belong to the days array and values lower or equal 0 to the daysBefore.
        /// </summary>
        /// <param name="day">Values from minus daysBefore.Length-1 to days.Length</param>
        public Day this[int day]
        {
            get
            {
                if (day < (daysBefore.Length - 1) * -1 || day > days.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(day), "Day must betweeen daysBefore.Length-1 and days.Length.");
                }

                return day <= 0 ? daysBefore[Math.Abs(day)] : days[--day];
            }

            set
            {
                if (day < (daysBefore.Length - 1) * -1 || day > days.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(day), "Day must betweeen daysBefore.Length-1 and days.Length.");
                }

                if (day <= 0)
                {
                    daysBefore[Math.Abs(day)] = value;
                }
                else
                {
                    days[--day] = value;
                }
            }
        }

        /// <summary>
        /// Creates a Month object using existing arrrays
        /// </summary>
        /// <param name="daysBefore">Array that will be indexed with indices less than 1</param>
        /// <param name="days">Array that will be accessed with indices greater or equal 1</param>
        /// <returns>New Month object</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any array length is less than one</exception>
        public static Month FromArrays(Day[] daysBefore, Day[] days)
        {
            if (daysBefore.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(daysBefore), "Length should be greater than 0");
            }

            if (days.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(days), "Length should be greater than 0");
            }

            return new Month(days, daysBefore);
        }
    }
}
