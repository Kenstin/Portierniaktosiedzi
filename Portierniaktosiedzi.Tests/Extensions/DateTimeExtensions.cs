using System;
using System.Collections.Generic;
using System.Linq;
using Portierniaktosiedzi.Extensions;
using Portierniaktosiedzi.Tests.DataSources;
using Xunit;

namespace Portierniaktosiedzi.Tests.Extensions
{
    public class DateTimeExtensions
    {
        [Theory]
        [MemberData(nameof(DateTimeExtensionsDataSource.TestData), MemberType = typeof(DateTimeExtensionsDataSource))]
        public void GetSaturdaysAndSundays(DateTime month, List<DateTime> days)
        {
            var output = month.GetSaturdaysAndSundays().ToList();
            output.Sort(DateTime.Compare);
            Assert.Equal(days, output);
        }
    }
}
