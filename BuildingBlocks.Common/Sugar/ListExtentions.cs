using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Sugar
{
    public static class ListExtentions
    {
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <c>null</c>.</exception>
        public static TSource Second<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.ElementAt(1);
        }

        /// <exception cref="ArgumentNullException"><paramref name="source" /> is <c>null</c>.</exception>
        public static TSource Third<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.ElementAt(2);
        }
    }
}