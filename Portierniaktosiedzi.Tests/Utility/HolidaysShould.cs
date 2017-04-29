using System;
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
    }
}
