using System.Linq;

namespace Portierniaktosiedzi.Models
{
    public class Day
    {
        public Employee[] Shifts { get; } = new Employee[3];

        /// <summary>
        /// Checks whether all shifts are full
        /// </summary>
        /// <returns>True if all shifts are full, false otherwise</returns>
        public bool IsFull() => Shifts.All(employee => employee != null);
    }
}
