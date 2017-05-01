using System;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Utility
{
    public interface IHolidays
    {
        /// <summary>
        /// Gets an IEnumerable of every work-free holiday in the given year
        /// </summary>
        IEnumerable<DateTime> HolidayDates { get; }
    }
}
