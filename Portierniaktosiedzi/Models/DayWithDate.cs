using System;
using System.Collections.ObjectModel;
using Portierniaktosiedzi.Extensions;

namespace Portierniaktosiedzi.Models
{
    public class DayWithDate : Day
    {
        public DayWithDate(DateTime date)
        {
            Date = date.Clone();
        }

        public new ObservableCollection<Employee> Shifts { get; } = new ObservableCollection<Employee> { null, null, null };

        public DateTime Date { get; }
    }
}