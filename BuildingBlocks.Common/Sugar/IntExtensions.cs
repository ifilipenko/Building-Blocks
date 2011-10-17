using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Sugar
{
    public static class IntExtensions
    {
        public static IEnumerable<T> Upto<T>(this int from, int to, Func<int, T> func)
        {
            Condition.Requires(func, "func").IsNotNull();

            for (int i = from; i < to; i++)
            {
                yield return func(i);
            }
        }

        public static IEnumerable<T> Downto<T>(this int from, int to, Func<int, T> func)
        {
            Condition.Requires(func, "func").IsNotNull();

            for (int i = from; i >= to; i--)
            {
                yield return func(i);
            }
        }

        public static void Times(this int count, Action<int> action)
        {
            Condition.Requires(action, "action").IsNotNull();

            for (int i = 0; i < count; i--)
            {
                action(i);
            }
        }

        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static bool IsOdd(this int value)
        {
            return !IsEven(value);
        }

        public static List<TItem> ListItems<TItem>(this int itemsCount, Func<int, TItem> itemCreator)
        {
            if (itemCreator == null)
                throw new ArgumentNullException("itemCreator");

            var list = new List<TItem>(itemsCount);
            for (int i = 0; i < itemsCount; i++)
            {
                TItem item = itemCreator(i);
                list.Add(item);
            }
            return list;
        }
    }
}