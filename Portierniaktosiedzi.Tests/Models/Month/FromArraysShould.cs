using System;
using Portierniaktosiedzi.Models;
using Xunit;

namespace Portierniaktosiedzi.Tests.Models.Month
{
    public class FromArraysShould
    {
        [Fact]
        public void ThrowExceptionGivenArrayLengthsLowerThanOne()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Portierniaktosiedzi.Models.Month.FromArrays(new Day[0], new Day[1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => Portierniaktosiedzi.Models.Month.FromArrays(new Day[1], new Day[0]));
        }
    }
}
