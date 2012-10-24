using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common
{
    public static class ObjectHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">value</exception>
        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");
            yield return value;
        }

        public static IEnumerable<T> ToEnumerableOrEmpty<T>(this T value)
        {
            if (!ReferenceEquals(value, null))
            {
                yield return value;
            }
        }
    }
}