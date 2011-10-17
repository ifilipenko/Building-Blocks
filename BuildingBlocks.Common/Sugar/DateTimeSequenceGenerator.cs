using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Sugar
{
    public class DateTimeSequenceGenerator
    {
        private readonly DateTime _from;
        private readonly int _count;

        public DateTimeSequenceGenerator(DateTime from, int count)
        {
            _from = from;
            _count = count;
        }

        public IEnumerable<DateTime> Days
        {
            get
            {
                return Enumerable.Range(0, _count)
                    .Select(x => _from.AddDays(x));
            }
        }
    }
}