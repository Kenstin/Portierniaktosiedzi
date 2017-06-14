using System;
using Portierniaktosiedzi.Extensions;

namespace Portierniaktosiedzi.Models
{
    public class DayWithDate : Day
    {
        public DayWithDate(DateTime date)
        {
            Date = date.Clone();
        }

        public DateTime Date { get; }
    }
}