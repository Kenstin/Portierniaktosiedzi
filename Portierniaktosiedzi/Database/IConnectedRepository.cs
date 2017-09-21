using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Database
{
    public interface IConnectedRepository
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Accesses Db")]
        ObservableCollection<Employee> GetEmployees();

        Task SaveChangesAsync();
    }
}
