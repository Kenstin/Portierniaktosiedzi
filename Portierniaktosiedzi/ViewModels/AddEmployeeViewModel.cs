using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.ViewModels
{
    public class AddEmployeeViewModel : Caliburn.Micro.Screen
    {
        public string EmployeeName { get; set; }

        public decimal Posts { get; set; } = 1;

        public Gender Gender { get; set; }

        public bool CanOk => !string.IsNullOrWhiteSpace(EmployeeName);

        public void Ok()
        {
            EmployeeName = EmployeeName.Trim();
            TryClose(true);
        }
    }
}