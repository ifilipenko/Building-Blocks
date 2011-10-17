using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildingBlocks.Common.Utils
{
    public delegate void ChildrenAction<T>(T parent, T child);

    public static class CollectionUtil
    {
        public static void AddIfNotContains<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static bool IsLastIndex<T>(this IEnumerable<T> list, int index)
        {
            return index == list.Count() - 1;
        }

        // todo: убрать сконвертировано в .net 4.0
        public static string JoinToString(this IEnumerable<string> enumerable, string separator)
        {
            if (enumerable == null)
                return null;
            // todo: убрать ToArray когда будет сконвертировано в .net 4.0
            return string.Join(separator, enumerable.ToArray());
        }

        public static string JoinToString(this IEnumerable<object> enumerable, string separator)
        {
            if (enumerable == null)
                return null;
            // todo: убрать ToArray когда будет сконвертировано в .net 4.0
            return string.Join(separator, enumerable.Select(i => (i ?? "<null>").ToString()).ToArray());
        }

        public static T AddOrUpdate<T, TKey, TValue>(this T dictionary, TKey key, TValue value)
            where T : IDictionary<TKey, TValue>
        {
            try
            {
                dictionary[key] = value;
            }
            catch (KeyNotFoundException)
            {
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
        {
            if (enumerable is IList<T>)
            {
                return ((IList<T>) enumerable).IndexOf(item);
            }

            int i = 0;
            foreach (var element in enumerable)
            {
                if (Equals(element, item))
                    return i;
                i++;
            }
            return -1;
        }

        public static int FindIndex<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var element in enumerable)
            {
                if (predicate(element))
                    return i;
                i++;
            }
            return -1;
        }

        public static int FindIndexAs<T>(this IEnumerable enumerable, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var element in enumerable)
            {
                if (predicate((T) element))
                    return i;
                i++;
            }
            return -1;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static bool IsNullOrEmpty(this IEnumerable enumerable)
        {
            return enumerable == null || enumerable.OfType<object>().Count() == 0;
        }

        public static IList<T> AsIList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable is IList<T> ? (IList<T>)enumerable : enumerable.ToList();
        }

        public static List<T> AsList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable is List<T> ? (List<T>)enumerable : enumerable.ToList();
        }

        public static T SafeGetItemAtIndex<T>(this IList<T> items, int index)
        {
            if (index < 0 || items == null || items.Count == 0)
            {
                return default(T);
            }

            return index < items.Count ? items[index] : items[index%items.Count];
        }

        public static string ItemsToString(this IEnumerable collection, string separator, 
                                           Func<object, string> itemToString)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (itemToString == null)
            {
                throw new ArgumentNullException("itemToString");
            }

            StringBuilder result = new StringBuilder();
            foreach (object item in collection)
            {
                result.Append(itemToString(item));
                result.Append(separator);
            }
            if (result.Length > 0 && !string.IsNullOrEmpty(separator))
            {
                result.Length -= separator.Length;
            }
            return result.ToString();
        }

        public static string ItemsToString(this IEnumerable collection, string separator)
        {
            Func<object, string> itemToString =
                delegate(object item) { return item == null ? "null" : item.ToString(); };
            return ItemsToString(collection, separator, itemToString);
        }

        public static bool SafeEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            if (list1 != null && list2 == null)
                return false;
            if (list1 == null && list2 != null)
                return false;
            if (ReferenceEquals(list1, list2))
                return true;

            return list1.SequenceEqual(list2);
        }

        public static bool NotContainDoublicates<TItem>(this IEnumerable<TItem> items)
        {
            var prevItem = default(TItem);
            foreach (var item in items.OrderBy(i => i))
            {
                if (!Equals(prevItem, default(TItem)) && Equals(prevItem, item))
                {
                    return false;
                }
                prevItem = item;
            }
            return true;
        }

        public static bool SafeEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
        {
            if (list1 != null && list2 == null)
                return false;
            if (list1 == null && list2 != null)
                return false;
            if (ReferenceEquals(list1, list2))
                return true;

            return list1.SequenceEqual(list2, comparer);
        }

        public static bool MoveItem<T>(this IList<T> list, T item, int movingPostions)
        {
            int currentPosition = list.IndexOf(item);
            int newPosition = currentPosition + movingPostions;
            int increment = movingPostions > 0 ? 1 : -1;

            if (currentPosition < 0)
            {
                return false;
            }

            if (newPosition < 0)
            {
                newPosition = 0;
            }
            else if (newPosition >= list.Count)
            {
                newPosition = list.Count - 1;
            }

            if (newPosition == currentPosition)
            {
                return false;
            }

            T movedItem = list[currentPosition];
            for (int i = currentPosition + increment; i != newPosition + increment; i += increment)
            {
                T nextItem = list[i];
                list[i - increment] = nextItem;
            }
            list[newPosition] = movedItem;

            return true;
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
        {
            foreach (TItem item in source)
            {
                action(item);
            }
        }

        public static void ForEach(this IEnumerable values, Action<object> action)
        {
            foreach (object item in values)
            {
                action(item);
            }
        }

        public static void HierarhicalForEach<T>(this IEnumerable<T> list,
            Func<T, IEnumerable<T>> childrenGetter, 
            ChildrenAction<T> action)
        {
            if (childrenGetter == null)
                throw new ArgumentNullException("childrenGetter");
            if (action == null)
                throw new ArgumentNullException("action");

            HierarhicalForEachCore<T>(list, default(T), childrenGetter, action);
        }

        private static void HierarhicalForEachCore<T>(IEnumerable<T> items, T parent, Func<T, IEnumerable<T>> childrenGetter, ChildrenAction<T> action)
        {
            if (items == null)
                return;

            foreach (T item in items)
            {
                action(parent, item);
                IEnumerable<T> children = childrenGetter(item);
                HierarhicalForEachCore(children, item, childrenGetter, action);
            }
        }
    }
}