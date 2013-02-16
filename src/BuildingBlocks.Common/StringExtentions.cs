using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common
{
    public static class StringExtentions
    {
        /// <summary>
        /// Truncates the string if its length exceeds the specified number of characters 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="limitString"></param>
        /// <param name="ellipsis"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">elipsis string is null</exception>
        /// <exception cref="ArgumentException">elipsis string has invalid lenght</exception>
        public static string EllipsisIfLongerThen(this string value, int limitString, string ellipsis = "...")
        {
            if (ellipsis == null)
                throw new ArgumentNullException("ellipsis");
            if (ellipsis.Length > value.Length || ellipsis.Length >= limitString)
                throw new System.ArgumentException("elipsis has invalid lenght", "ellipsis");

            if (string.IsNullOrEmpty(value) || value.Length <= limitString)
                return value;

            return value.Substring(0, limitString - ellipsis.Length) + ellipsis;
        }

        public static string JoinToString(this IEnumerable<string> strings, string separator = ",")
        {
            return string.Join(separator, strings);
        }
    }
}