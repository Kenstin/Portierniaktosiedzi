using System;
using System.Collections.ObjectModel;

namespace Portierniaktosiedzi.Utility
{
    public interface IHolidays
    {
        DateTime CalculateEasterDate();

        Collection<DateTime> GetHolidays();
    }
}
