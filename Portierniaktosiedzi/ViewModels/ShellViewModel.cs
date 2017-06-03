using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly IWindowManager windowManager;
        private readonly List<Employee> employees;

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            employees = new List<Employee>();
        }

        public void AddEmployee()
        {
            var employeeViewModel = new AddEmployeeViewModel();
            if (windowManager.ShowDialog(employeeViewModel) ?? false)
            {
                employees.Add(new Employee(employeeViewModel.Posts, employeeViewModel.Gender, employeeViewModel.EmployeeName));
            }
        }

        public bool GenerateTimetable()
        {
            throw new NotImplementedException();
        }
    }
}