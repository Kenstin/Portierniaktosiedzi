using System;
using Portierniaktosiedzi.Models;
using Xunit;

namespace Portierniaktosiedzi.Tests.Models.Month
{
    public class MonthIndexerShould
    {
        private readonly Day[] days;
        private readonly Day[] daysBefore;

        public MonthIndexerShould()
        {
            days = new Day[10];
            daysBefore = new Day[3];
        }

        [Fact]
        public void ThrowExceptionGivenValuesOutOfRange()
        {
            var month = Portierniaktosiedzi.Models.Month.FromArrays(daysBefore, days);
            Assert.Throws<ArgumentOutOfRangeException>(() => month[-3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => month[11]);
        }

        [Fact]
        public void ThrowExceptionSettingValuesOutOfRange()
        {
            var month = Portierniaktosiedzi.Models.Month.FromArrays(daysBefore, days);
            Assert.Throws<ArgumentOutOfRangeException>(() => month[-3] = new Day());
            Assert.Throws<ArgumentOutOfRangeException>(() => month[11] = new Day());
        }

        [Fact]
        public void ReturnDaysGivenValuesGreaterThanZero()
        {
            days[0] = new Day(); //1. day
            days[1] = new Day(); //2. day
            daysBefore[0] = new Day(); //0. day
            daysBefore[1] = new Day(); //-1. day

            var month = Portierniaktosiedzi.Models.Month.FromArrays(daysBefore, days);
            Assert.Equal(days[0], month[1]);
            Assert.Equal(days[1], month[2]);
        }

        [Fact]
        public void ReturnDaysBeforeGivenValuesLowerOrEqualZero()
        {
            days[0] = new Day(); //1. day
            days[1] = new Day(); //2. day
            daysBefore[0] = new Day(); //0. day
            daysBefore[1] = new Day(); //-1. day

            var month = Portierniaktosiedzi.Models.Month.FromArrays(daysBefore, days);
            Assert.Equal(daysBefore[0], month[0]);
            Assert.Equal(daysBefore[1], month[-1]);
        }

        [Fact]
        public void SetDaysGivenValuesGreaterThanZero()
        {
            days[0] = new Day(); //1. day
            days[1] = new Day(); //2. day
            daysBefore[0] = new Day(); //0. day
            daysBefore[1] = new Day(); //-1. day

            var month = Portierniaktosiedzi.Models.Month.FromArrays(new Day[daysBefore.Length], new Day[days.Length]);
            month[1] = days[0];
            month[2] = days[1];
            Assert.Equal(days[0], month[1]);
            Assert.Equal(days[1], month[2]);
        }

        [Fact]
        public void SetDaysBeforeGivenValuesLowerOrEqualZero()
        {
            days[0] = new Day(); //1. day
            days[1] = new Day(); //2. day
            daysBefore[0] = new Day(); //0. day
            daysBefore[1] = new Day(); //-1. day

            var month = Portierniaktosiedzi.Models.Month.FromArrays(new Day[daysBefore.Length], new Day[days.Length]);
            month[0] = daysBefore[0];
            month[-1] = daysBefore[1];
            Assert.Equal(daysBefore[0], month[0]);
            Assert.Equal(daysBefore[1], month[-1]);
        }
    }
}
