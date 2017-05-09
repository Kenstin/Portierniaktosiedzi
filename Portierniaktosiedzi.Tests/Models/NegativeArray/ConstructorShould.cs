using System;
using Portierniaktosiedzi.Models;
using Xunit;

namespace Portierniaktosiedzi.Tests.Models.NegativeArray
{
    public class ConstructorShould
    {
        [Fact]
        public void ThrowExceptionGivenArrayLengthsLowerThanOne()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new NegativeArray<Day>(new Day[0], new Day[1]));
            Assert.Throws<ArgumentOutOfRangeException>(() => new NegativeArray<Day>(new Day[1], new Day[0]));
        }
    }
}
