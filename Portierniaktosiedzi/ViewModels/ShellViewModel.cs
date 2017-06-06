using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.ViewModels
{
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly IWindowManager windowManager;

        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            Employees = new BindableCollection<Employee>();
            Days = new BindableCollection<DayWithDate>();
        }

        public DateTime Date { get; set; } = DateTime.Now;

        public BindableCollection<Employee> Employees { get; }

        public BindableCollection<DayWithDate> Days { get; }

        public void AddEmployee()
        {
            var employeeViewModel = new AddEmployeeViewModel();
            if (windowManager.ShowDialog(employeeViewModel) ?? false)
            {
                Employees.Add(new Employee(employeeViewModel.Posts, employeeViewModel.Gender, employeeViewModel.EmployeeName));
            }
        }

        public bool GenerateTimetable()
        {
            throw new NotImplementedException();
        }
    }
}