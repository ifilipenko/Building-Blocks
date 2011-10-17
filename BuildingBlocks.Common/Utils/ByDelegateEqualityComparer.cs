using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils
{
    public class ByDelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly System.Func<T, T, bool> _eqaulityCompareMethod;

        public ByDelegateEqualityComparer(System.Func<T, T, bool> eqaulityCompareMethod)
        {
            _eqaulityCompareMethod = eqaulityCompareMethod;
        }

        #region Implementation of IEqualityComparer<T>

        public bool Equals(T x, T y)
        {
            return _eqaulityCompareMethod(x, y);
        }

        public int GetHashCode(T obj)
        {
            return Object.Equals(obj, default(T)) ? 0 : obj.GetHashCode();
        }

        #endregion
    }
}