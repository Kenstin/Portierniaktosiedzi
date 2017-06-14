using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Win32;
using Portierniaktosiedzi.Models;
using Portierniaktosiedzi.Utility;

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

            var schoolEmployee = new SchoolStaff();
            ComboBoxEmployees.Add(schoolEmployee);

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
            var dialog = new SaveFileDialog
            {
                Filter = "Arkusz (*.xlsx)|*.xlsx",
                FileName = "Harmonogoram " + CultureInfo.CreateSpecificCulture("pl").DateTimeFormat
                               .GetMonthName(Date.Month),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            dialog.ShowDialog();

            var timetable = new Timetable(Date, Days);
            timetable.Generate(Employees, new Holidays(Date.Year));
            using (var saveAsXlsx = new SaveAsXlsx(timetable))
            {
                saveAsXlsx.SaveAs(dialog.FileName);
            }

            return true;
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