using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Portierniaktosiedzi.Models;
using Portierniaktosiedzi.Utility;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Portierniaktosiedzi.ViewModels
{
    public sealed class ShellViewModel : PropertyChangedBase, IShell
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

            Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var schoolEmployee = new SchoolStaff();
            ComboBoxEmployees.Add(schoolEmployee);
        }

        public bool GeneratingTimetable { get; set; }

        public DateTime Date { get; set; }

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

        public async Task GenerateTimetable()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Arkusz (*.xlsx)|*.xlsx",
                FileName = "Harmonogoram " + CultureInfo.CreateSpecificCulture("pl").DateTimeFormat
                               .GetMonthName(Date.Month),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            GeneratingTimetable = true;
            if ((dialog.ShowDialog() ?? false) && await TryGeneratingTimetable(dialog.FileName))
            {
                GeneratingTimetable = false;
                MessageBox.Show("Poprawnie utworzono i zapisano harmonogram");
            }
            else
            {
                GeneratingTimetable = false;
                MessageBox.Show("Nie da sie wygenerowac harmonogramu z obecnymi ustawieniami.");
            }
        }

        private async Task<bool> TryGeneratingTimetable(string path)
        {
            Timetable timetable;
            Task<bool> generation;
            try
            {
                timetable = new Timetable(Date, Days);
                generation = Task.Run(() => timetable.Generate(Employees, new Holidays(Date.Year)));
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Prosze uzupelnic wszystkie zmiany.");
                return false;
            }

            var savingTask = Task.Run(async () =>
            {
                var saveAsXlsx = new SaveAsXlsx(timetable);
                try
                {
                    if (await generation.ConfigureAwait(false))
                    {
                        saveAsXlsx.SaveAs(path);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Plik jest juz w uzyciu.");
                    return false;
                }
                finally
                {
                    saveAsXlsx.Dispose();
                }
                return true;
            });

            return await savingTask;
        }

        private void OnDateChanged() //it is used, because of PropertyChanged.Fody
        {
            Days.Clear();
            var currDate = Date.AddDays(-1);
            for (var i = 0; i < 6; i++)
            {
                Days.Add(new DayWithDate(currDate));
                currDate = currDate.AddDays(-1);
            }
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
