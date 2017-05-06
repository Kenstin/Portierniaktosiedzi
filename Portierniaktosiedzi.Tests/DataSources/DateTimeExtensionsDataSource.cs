using System;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Tests.DataSources
{
    public static class DateTimeExtensionsDataSource
    {
        private static readonly List<object[]> Data
            = new List<object[]>
            {
                new object[]
                {
                    new DateTime(2017, 5, 1),
                    new List<DateTime>
                    {
                        new DateTime(2017, 05, 6),
                        new DateTime(2017, 05, 7),
                        new DateTime(2017, 05, 13),
                        new DateTime(2017, 05, 14),
                        new DateTime(2017, 05, 20),
                        new DateTime(2017, 05, 21),
                        new DateTime(2017, 05, 27),
                        new DateTime(2017, 05, 28)
                    }
                },
                new object[]
                {
                    new DateTime(2017, 6, 1),
                    new List<DateTime>
                    {
                        new DateTime(2017, 06, 3),
                        new DateTime(2017, 06, 4),
                        new DateTime(2017, 06, 10),
                        new DateTime(2017, 06, 11),
                        new DateTime(2017, 06, 17),
                        new DateTime(2017, 06, 18),
                        new DateTime(2017, 06, 24),
                        new DateTime(2017, 06, 25)
                    }
                }
            };

        public static IEnumerable<object[]> TestData => Data;
    }
}
