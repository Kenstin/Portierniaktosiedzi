using System;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Extensions
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> GetSaturdaysAndSundays(this DateTime dateTime)
        {
            var freeDays = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month); i++)
            {
                var day = new DateTime(dateTime.Year, dateTime.Month, i);
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    freeDays.Add(day);
                }
            }

            return freeDays;
        }

        public static DateTime Clone(this DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        public static IEnumerable<Tuple<DateTime, DateTime>> GetWeekendTuples(this DateTime date)
        {
            foreach (var day in date.GetSaturdaysAndSundays())
            {
                if (day.DayOfWeek == DayOfWeek.Saturday && day.AddDays(1).DayOfWeek == DayOfWeek.Sunday)
                {
                    yield return Tuple.Create(day, day.AddDays(1));
                }
            }
        }
    }
}
