using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.Utils
{
	// ReSharper disable UnusedMember.Global
	public static class AnonymousComparer
	{
		// IComparer<T>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, 
																		 Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.OrderBy(keySelector, CreateComparer(compare));
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.OrderByDescending(keySelector, CreateComparer(compare));
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.ThenBy(keySelector, CreateComparer(compare));
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.ThenByDescending(keySelector, CreateComparer(compare));
		}

		// IEqualityComparer<T>
		public static bool Contains<TSource, TCompareKey>(this IEnumerable<TSource> source, TSource value, Func<TSource, TCompareKey> compareKeySelector)
		{
			return source.Contains(value, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TSource> Distinct<TSource, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TCompareKey> compareKeySelector)
		{
			return source.Distinct(CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TSource> Except<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Except(second, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, resultSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, elementSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TSource> Intersect<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Intersect(second, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static bool SequenceEqual<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.SequenceEqual(second, CreateEqualityComparer(compareKeySelector));
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.ToDictionary(keySelector, elementSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.ToLookup(keySelector, CreateEqualityComparer(compareKeySelector));
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.ToLookup(keySelector, elementSelector, CreateEqualityComparer(compareKeySelector));
		}

		public static IEnumerable<TSource> Union<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Union(second, CreateEqualityComparer(compareKeySelector));
		}

		private static IComparer<TKey> CreateComparer<TKey>(Func<TKey, TKey, int> compare)
		{
			return ComparerFactory.Create(compare);
		}

		private static IEqualityComparer<TSource> CreateEqualityComparer<TSource, TKey>(Func<TSource, TKey> compareKeySelector)
		{
			return ComparerFactory.Create(compareKeySelector);
		}
	}
	// ReSharper restore UnusedMember.Global
}