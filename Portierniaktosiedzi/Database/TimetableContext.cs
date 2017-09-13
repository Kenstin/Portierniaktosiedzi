using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Database
{
    public class TimetableContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var source = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Portierniaktosiedzi/Portiernia.db");
            optionsBuilder.UseSqlite("Data Source=" + source);
        }
    }
}
