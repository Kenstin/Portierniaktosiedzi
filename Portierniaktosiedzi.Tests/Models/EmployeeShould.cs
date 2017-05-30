using Portierniaktosiedzi.Models;
using Portierniaktosiedzi.Utility;
using Xunit;

namespace Portierniaktosiedzi.Tests.Models
{
    public class EmployeeShould
    {
        [Theory]
        [InlineData(6, 2017, 1, 168)]
        [InlineData(8, 2017, 1, 176)]
        [InlineData(12, 2017, 1, 152)]
        [InlineData(4, 2017, 0.5, 76)]
        [InlineData(4, 2017, 0.6, 91.2)]
        [InlineData(11, 2017, 1, 168)]
        public void GetWorkingHours(int month, int year, decimal posts, decimal expected)
        {
            var employee = new Employee(posts, Gender.Man, "Placeholder");
            Assert.Equal(expected, employee.GetWorkingHours(month, year, new Holidays(year)));
        }
    }
}
