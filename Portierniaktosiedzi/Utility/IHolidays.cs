using System;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Utility
{
    public interface IHolidays
    {
        IEnumerable<DateTime> HolidayDates { get; }
    }
}
