using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Portierniaktosiedzi.Database;
using Portierniaktosiedzi.Exceptions;
using Portierniaktosiedzi.Models;
using Portierniaktosiedzi.Utility;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Portierniaktosiedzi.ViewModels
{
    public sealed class ShellViewModel : PropertyChangedBase, IShell
    {
        private readonly IWindowManager windowManager;
        private readonly IConnectedRepository repository;

        public ShellViewModel(IWindowManager windowManager, IConnectedRepository repository)
        {
            this.windowManager = windowManager;
            this.repository = repository;
            Employees = repository.GetEmployees();
            Days = new BindableCollection<DayWithDate>();
            DeletedEmployees = new BindableCollection<Employee>();
            ComboBoxEmployees = new BindableCollection<Employee>(Employees);
            Employees.CollectionChanged += EmployeesOnCollectionChanged;

            Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var schoolEmployee = new SchoolStaff();
            ComboBoxEmployees.Add(schoolEmployee);
        }

        public bool GeneratingTimetable { get; set; }

        public DateTime Date { get; set; }

        public ObservableCollection<Employee> Employees { get; }

        public BindableCollection<Employee> ComboBoxEmployees { get; }

        public Employee SelectedEmployee { get; set; }

        public BindableCollection<Employee> DeletedEmployees { get; }

        public BindableCollection<DayWithDate> Days { get; }

        public async Task AddEmployee()
        {
            var employeeViewModel = new AddEmployeeViewModel();

            dynamic settings = new ExpandoObject();
            settings.Title = "Dodaj pracownika";

            if (windowManager.ShowDialog(employeeViewModel, settings: settings) ?? false)
            {
                var employee = new Employee(employeeViewModel.Posts, employeeViewModel.Gender, employeeViewModel.EmployeeName);
                Employees.Add(employee);
                await repository.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployee()
        {
            if (SelectedEmployee != null && !DeletedEmployees.Contains(SelectedEmployee))
            {
                DeletedEmployees.Add(SelectedEmployee);
                Employees.Remove(SelectedEmployee);
                await repository.SaveChangesAsync();
                ComboBoxEmployees.Refresh();
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

            try
            {
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    if (await TryGeneratingTimetable(dialog.FileName).ConfigureAwait(false))
                    {
                        MessageBox.Show("Poprawnie utworzono i zapisano harmonogram");
                    }
                    else
                    {
                        MessageBox.Show("Nie da sie wygenerowac harmonogramu z obecnymi ustawieniami.");
                    }
                }
            }
            catch (ShiftsNotAssignedException)
            {
                MessageBox.Show("Prosze uzupelnic wszystkie zmiany.");
            }
            catch (IOException)
            {
                MessageBox.Show("Plik jest juz w uzyciu.");
            }
            finally
            {
                GeneratingTimetable = false;
            }
        }

        private async Task<bool> TryGeneratingTimetable(string path)
        {
            var timetable = new Timetable(Date, Days);
            var generation = Task.Run(() => timetable.Generate(Employees, new Holidays(Date.Year)));

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
                catch (IOException e)
                {
                    throw new IOException("Plik jest juz w uzyciu.", e);
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
