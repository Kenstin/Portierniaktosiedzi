using System.Collections.ObjectModel;
using System.Linq;

namespace Portierniaktosiedzi.Models
{
    public class Day
    {
        public ObservableCollection<Employee> Shifts { get; } = new ObservableCollection<Employee> { null, null, null };

        /// <summary>
        /// Checks whether all shifts are full
        /// </summary>
        /// <returns>True if all shifts are full, false otherwise</returns>
        public bool IsFull() => Shifts.All(employee => employee != null);

        public bool IsThere(Employee employee) => Shifts.Any(e => e == employee);

        public override string ToString()
        {
            return $"0: {Shifts[0]?.Name ?? "***"} 1: {Shifts[1]?.Name ?? "***"} 2: {Shifts[2]?.Name ?? "***"}";
        }
    }
}