using Portierniaktosiedzi.Models;
using Portierniaktosiedzi.Utility;
using Xunit;

namespace Portierniaktosiedzi.Tests.Models
{
    public class EmployeeShould
    {
        [Theory]
        [InlineData(6, 2017, 168)]
        [InlineData(8, 2017, 176)]
        [InlineData(12, 2017, 152)]
        public void GetWorkingHours(int month, int year, int expected)
        {
            var employee = new Employee(1, Gender.Man, "Placeholder");
            Assert.Equal(expected, employee.GetWorkingHours(month, year, new Holidays(year)));
        }

        [Fact]
        public void GetWorkingHoursShouldBeDivisibleBy8()
        {
            var employee = new Employee(0.5f, Gender.Man, "Placeholder");
            Assert.Equal(72, employee.GetWorkingHours(4, 2017, new Holidays(2017)));
            employee = new Employee(0.6f, Gender.Man, "Placeholder");
            Assert.Equal(88, employee.GetWorkingHours(4, 2017, new Holidays(2017)));
        }
    }
}
