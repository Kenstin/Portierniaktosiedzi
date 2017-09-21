using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Database
{
    public class TimetableContext : DbContext
    {
        public TimetableContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var source = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Portierniaktosiedzi");
            Directory.CreateDirectory(source);
            source += "/Portiernia.db";
            if (!File.Exists(source))
            {
                File.Create(source).Dispose();
            }

            optionsBuilder.UseSqlite("Data Source=" + source);
        }
    }
}
