using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
            DeletedEmployees = new BindableCollection<Employee>();
            ComboBoxEmployees = new BindableCollection<Employee>();
            Employees.CollectionChanged += EmployeesOnCollectionChanged;
        }

        public DateTime Date { get; set; } = DateTime.Now;

        public BindableCollection<Employee> Employees { get; }

        public BindableCollection<Employee> ComboBoxEmployees { get; }

        public Employee SelectedEmployee { get; set; }

        public BindableCollection<Employee> DeletedEmployees { get; }

        public BindableCollection<DayWithDate> Days { get; }

        public void AddEmployee()
        {
            var employeeViewModel = new AddEmployeeViewModel();
            if (windowManager.ShowDialog(employeeViewModel) ?? false)
            {
                var employee = new Employee(employeeViewModel.Posts, employeeViewModel.Gender, employeeViewModel.EmployeeName);
                Employees.Add(employee);
            }
        }

        public void DeleteEmployee()
        {
            if (SelectedEmployee != null && !DeletedEmployees.Contains(SelectedEmployee))
            {
                DeletedEmployees.Add(SelectedEmployee);
                Employees.Remove(SelectedEmployee);
            }
        }

        public bool GenerateTimetable()
        {
            throw new NotImplementedException();
        }

        private void EmployeesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.NewItems != null)
            {
                ComboBoxEmployees.AddRange(notifyCollectionChangedEventArgs.NewItems.OfType<Employee>());
            }
        }
    }
}