using System;
using System.Collections.Generic;
using System.Linq;
using Portierniaktosiedzi.Utility;
using Xunit;

namespace Portierniaktosiedzi.Tests.Utility
{
    public class HolidaysShould
    {
        [Theory]
        [InlineData(2016, 3, 27)]
        [InlineData(2017, 4, 16)]
        [InlineData(2018, 4, 1)]
        public void CalculateEasterDate(int year, int outMonth, int outDay)
        {
            var holidays = new Holidays(year);
            Assert.True(DateTime.Equals(holidays.CalculateEasterDate(), new DateTime(year, outMonth, outDay)));
        }

        [Fact]
        public void ThrowArgOutOfRangeExceptionGivenYearBelow1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Holidays(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Holidays(int.MinValue));
        }

        [Fact]
        public void GetHolidaysIn2016()
        {
            int y = 2016;
            var holidays = new Holidays(y);
            var expectedList = new List<DateTime>
            {
                new DateTime(y, 1, 1),
                new DateTime(y, 1, 6),
                new DateTime(y, 3, 27),
                new DateTime(y, 3, 28),
                new DateTime(y, 5, 1),
                new DateTime(y, 5, 3),
                new DateTime(y, 5, 15),
                new DateTime(y, 5, 26),
                new DateTime(y, 8, 15),
                new DateTime(y, 11, 1),
                new DateTime(y, 11, 11),
                new DateTime(y, 12, 25),
                new DateTime(y, 12, 26)
            };

            var holidaysList = holidays.HolidayDates.ToList();
            holidaysList.Sort(DateTime.Compare);
            Assert.Equal(expectedList, holidaysList);
        }
    }
}
