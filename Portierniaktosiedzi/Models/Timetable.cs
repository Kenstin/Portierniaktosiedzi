using System;
using System.Collections.Generic;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Timetable
    {
        public Timetable(DateTime month, Day dayBefore)
        {
            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }

            Month = month;
            Days = new Day[DateTime.DaysInMonth(month.Year, month.Month) + 1];
            Days[0] = dayBefore ?? throw new ArgumentNullException(nameof(dayBefore)); /////////////////////

            //this.employees = employees.OrderBy(e => e.Posts).ToList();
            //this.holidays = holidays ?? throw new ArgumentNullException(nameof(holidays));

            for (int i = 1; i < Days.Length; i++)
            {
                Days[i] = new Day();
            }

            /*xD = new Dictionary<Employee, ushort>
            {
                { this.employees[0], 0 },
                { this.employees[1], 0 },
                { this.employees[2], 0 }
            };*/
        }

        public Day[] Days { get; }

        public DateTime Month { get; }

        //11 h - 2 zmiany odstepu
        //35h w tygodniu - 5 odstepu
        //wolny weekend raz na miesiac
        //5 dni w tygodniu max
        //5 dni pod rzad max

        public void Generate(IEnumerable<Employee> employees, IHolidays holidays)
        {
            var timetableGenerator = new TimetableGenerator(this, employees, holidays);
            timetableGenerator.Generate();
        }
    }
}
