using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils
{
    public static class EnumerableDebugger
    {
        public static ConsoleColor DefaultColor = ConsoleColor.Yellow;
        public static bool ShowWhiteSpace { get; set; }

        public static IEnumerable<T> Dump<T>(this IEnumerable<T> input)
        {
            return Dump(input, item => item != null ? item.ToString() : "(null)", DefaultColor);
        }

        public static IEnumerable<T> Dump<T>(this IEnumerable<T> input, ConsoleColor consoleColor)
        {
            return Dump(input, item => item != null ? item.ToString() : "(null)", consoleColor);
        }

        public static IEnumerable<T> Dump<T>(this IEnumerable<T> input, Func<T, string> toString, ConsoleColor consoleColor)
        {
            foreach (var item in input)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(ShowWhiteSpace ? '[' + toString(item) + ']' : toString(item));
                Console.ResetColor();
                yield return item;
            }
        }
    }
}