using System;
using System.Collections;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils
{
    public static class EnumeratorHelper
    {
        public static object Next(this IEnumerator enumerator, Func<object, object> processIfNotNull = null)
        {
            var result = NextCore(enumerator);
            return result != null && processIfNotNull != null ? processIfNotNull(result) : result;
        }

        public static T Next<T>(this IEnumerator<T> enumerator, Func<T, T> processIfNotNull = null, T defaultValue = default(T))
        {
            var result = NextCore(enumerator);
            return result == null
                       ? defaultValue
                       : (processIfNotNull == null ? (T) result : processIfNotNull((T) result));
        }

        public static T Next<T>(this IEnumerator enumerator, Func<T, T> processIfNotNull = null, T defaultValue = default(T))
        {
            var result = NextCore(enumerator);
            return result == null
                       ? defaultValue
                       : (processIfNotNull == null ? (T) result : processIfNotNull((T) result));
        }

        public static object SafeNext(this IEnumerator enumerator)
        {
            return SafeNextCore(enumerator);
        }

        public static T SafeNext<T>(this IEnumerator<T> enumerator, T defaultValue = default(T))
        {
            return (T)(SafeNextCore(enumerator) ?? defaultValue);
        }

        public static T SafeNext<T>(this IEnumerator enumerator, T defaultValue = default(T))
        {
            return (T)(SafeNextCore(enumerator) ?? defaultValue);
        }

        private static object NextCore(IEnumerator enumerator)
        {
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                return current;
            }
            return null;
        }

        private static object SafeNextCore(IEnumerator enumerator)
        {
            try
            {
                return NextCore(enumerator);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}