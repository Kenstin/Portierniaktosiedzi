using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Database
{
    public interface IConnectedRepository
    {
        ObservableCollection<Employee> GetEmployees();

        Task SaveChangesAsync();
    }
}
