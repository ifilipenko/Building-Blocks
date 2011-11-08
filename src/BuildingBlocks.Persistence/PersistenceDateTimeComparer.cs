using System;
using System.Collections.Generic;

namespace BuildingBlocks.Persistence
{
    public class PersistenceDateTimeComparer : IEqualityComparer<DateTime?>
    {
        public bool Equals(DateTime? x, DateTime? y)
        {
            var xAsString = x == null
                ? string.Empty
                : x.ToString();
            var yAsString = y == null
                ? string.Empty
                : y.ToString();

            return xAsString == yAsString;
        }

        public int GetHashCode(DateTime? obj)
        {
            return obj.GetHashCode();
        }
    }
}