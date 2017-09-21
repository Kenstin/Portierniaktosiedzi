using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Database
{
    public sealed class ConnectedRepository : IConnectedRepository, IDisposable
    {
        private readonly TimetableContext timetableContext;

        public ConnectedRepository()
        {
            timetableContext = new TimetableContext(new DbContextOptions<TimetableContext>());
            timetableContext.Database.Migrate();
        }

        public ObservableCollection<Employee> GetEmployees()
        {
            if (timetableContext.Employees.Local.Count == 0)
            {
                timetableContext.Employees.ToList();
            }

            return timetableContext.Employees.Local.ToObservableCollection();
        }

        public async Task SaveChangesAsync()
        {
            await timetableContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            timetableContext.Dispose();
        }
    }
}
