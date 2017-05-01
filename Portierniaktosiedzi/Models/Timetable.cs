using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portierniaktosiedzi.Utility;

namespace Portierniaktosiedzi.Models
{
    public class Timetable
    {
        private readonly Day dayBefore;
        private Day[] days;
        private List<Employee> employees;
        private readonly DateTime month;
        private readonly IHolidays holidays;

        public Timetable(DateTime month, Day dayBefore, IEnumerable<Employee> employees, IHolidays holidays)
        {
            if (month == null)
            {
                throw new ArgumentNullException(nameof(month));
            }

            this.month = month;
            days = new Day[DateTime.DaysInMonth(month.Year, month.Month)];
            this.dayBefore = dayBefore ?? throw new ArgumentNullException(nameof(dayBefore));
            this.employees = employees.ToList();
            this.holidays = holidays ?? throw new ArgumentNullException(nameof(holidays));
        }

        //11 h - 2 zmiany odstepu
        //35h w tygodniu - 5 odstepu
        //wolny weekend raz na miesiac
        //5 dni w tygodniu max
        public void Generate()
        {
            var workingHoursLeft = employees.ToDictionary(
                employee => employee, employee => employee.GetWorkingHours(month.Month, month.Year, holidays));

            var girls = employees.Where(e => e.Gender == Gender.Woman).ToList();
            var boys = employees.Where(e => e.Gender == Gender.Man).ToList();

            foreach (var day in days)
            {
                var first = boys.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
                workingHoursLeft[first] -= 8;
                day.Shifts[2] = first;
            }

            /*foreach (var day in days)
            {
                var first = girls.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
                workingHoursLeft[first] -= 8;
                day.Shifts[1] = first;
            }*/

            var employeesLeft = employees.Where(e => workingHoursLeft[e] > 0).ToList();

            var daysOff = new List<DateTime>();
            daysOff.AddRange(holidays.HolidayDates.Where(date => date.Month == month.Month));
            daysOff.AddRange(GetSaturdaysAndSundays());
            foreach (var date in daysOff)
            {
                days[date.Day].Shifts[1] = employeesLeft.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
                days[date.Day].Shifts[0] = employeesLeft.First(e => workingHoursLeft[e] > 0 && MeetsCriteria());
            }





        }

        private IEnumerable<DateTime> GetSaturdaysAndSundays()
        {
            var freeDays = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
            {
                var day = new DateTime(month.Year, month.Month, i);
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    freeDays.Add(day);
                }
            }

            return freeDays;
        }

        private bool MeetsCriteria()
        {
            throw new NotImplementedException();
        }
    }
}
