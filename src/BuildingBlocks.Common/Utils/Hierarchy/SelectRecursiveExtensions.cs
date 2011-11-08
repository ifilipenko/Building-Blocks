using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Utils.Hierarchy
{
    public static class SelectRecursiveExtensions
    {
        /// <summary>
        /// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
        /// and flattens the resulting sequences into one sequence. 
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements 
        /// who are the result of invoking the recursive transform function on each element of the input sequence. 
        /// </returns>
        /// <example>
        /// node.ChildNodes.SelectRecursive(n => n.ChildNodes);
        /// </example>
        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return SelectRecursive(source, selector, null);
        }

        /// <summary>
        /// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
        /// and flattens the resulting sequences into one sequence. 
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of 
        /// invoking the recursive transform function on each element of the input sequence. 
        /// </returns>
        /// <example>
        /// node.ChildNodes.SelectRecursive(n => n.ChildNodes, m => m.Depth < 2);
        /// </example>
        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate)
        {
            return SelectRecursive(source, selector, predicate, 0);
        }

        private static IEnumerable<IRecursion<T>> SelectRecursive<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate, int depth)
        {
            var q = source
                .Select(item => new Recursion<T>(depth, item))
                .Cast<IRecursion<T>>();
            if (predicate != null)
                q = q.Where(predicate);
            foreach (var item in q)
            {
                yield return item;
                foreach (var item2 in SelectRecursive(selector(item.Item), selector, predicate, depth + 1))
                    yield return item2;
            }
        }

        private class Recursion<T> : IRecursion<T>
        {
            private readonly int _depth;
            private readonly T _item;

            public Recursion(int depth, T item)
            {
                _depth = depth;
                _item = item;
            }

            public int Depth
            {
                get { return _depth; }
            }

            public T Item
            {
                get { return _item; }
            }
        }
    }
}