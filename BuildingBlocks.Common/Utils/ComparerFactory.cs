using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils
{
    public class ComparerFactory
    {
        public static IComparer<T> Create<T>(Func<T, T, int> compare)
        {
            if (compare == null)
                throw new ArgumentNullException("compare");

            return new Comparer<T>(compare);
        }

        public static IEqualityComparer<T> Create<T, TKey>(Func<T, TKey> compareKeySelector)
        {
            if (compareKeySelector == null) throw new ArgumentNullException("compareKeySelector");

            return new EqualityComparer<T>(
                (x, y) =>
                {
                    if (ReferenceEquals(x, y))
                        return true;
                    if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                        return false;
                    return compareKeySelector(x).Equals(compareKeySelector(y));
                },
                obj =>
                {
                    if (ReferenceEquals(obj, null))
                        return 0;
                    return compareKeySelector(obj).GetHashCode();
                });
        }

        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            if (equals == null)
                throw new ArgumentNullException("equals");
            if (getHashCode == null)
                throw new ArgumentNullException("getHashCode");

            return new EqualityComparer<T>(equals, getHashCode);
        }

        private class EqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equals;
            private readonly Func<T, int> _getHashCode;

            public EqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
            {
                _equals = equals;
                _getHashCode = getHashCode;
            }

            public bool Equals(T x, T y)
            {
                return _equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _getHashCode(obj);
            }
        }

        private class Comparer<T> : IComparer<T>
        {
            private readonly Func<T, T, int> _compare;

            public Comparer(Func<T, T, int> compare)
            {
                _compare = compare;
            }

            public int Compare(T x, T y)
            {
                return _compare(x, y);
            }
        }
    }
}