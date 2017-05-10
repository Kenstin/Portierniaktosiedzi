using Xunit;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Tests.Models.NegativeArray
{
    public class Enumerable
    {
        [Fact]
        public void GetEnumerator()
        {
            var array = new NegativeArray<int>(new int[5], new int[2]);
            array[2] = 2137;
            array[-4] = 2137;

            int sum = 0;
            foreach (var i in array)
            {
                sum += i;
            }
            Assert.Equal(4274, sum);
        }
    }
}
