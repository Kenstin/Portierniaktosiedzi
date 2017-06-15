using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class SchoolStaff : Employee
    {
        public SchoolStaff()
            : base(decimal.MaxValue, Gender.Woman, "Pracownik szkoly")
        {
        }

        public override decimal GetWorkingHours(int month, int year, IHolidays holidays)
        {
            return decimal.MaxValue;
        }
    }
}
