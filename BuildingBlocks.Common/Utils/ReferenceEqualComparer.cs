using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils
{
    public class ReferenceEqualComparer<T> : IEqualityComparer<T>
    {
        public static ReferenceEqualComparer<T> CreateInverseComparer()
        {
            return new ReferenceEqualComparer<T>(true);
        }

        public static ReferenceEqualComparer<T> Create()
        {
            return new ReferenceEqualComparer<T>(false);
        }

        private readonly bool _inverse;

        ReferenceEqualComparer(bool inverse)
        {
            _inverse = inverse;
        }

        public bool Equals(T x, T y)
        {
            return _inverse ? !ReferenceEquals(x, y) : ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}