using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Sugar
{
    public static class StringExtensions
    {
        public static string Reduce(this string value, int removeFromBegin, int removeFromEnd)
        {
            Condition.Requires(value, "value").IsNotNullOrEmpty();
            Condition.Requires(removeFromBegin, "removeFromBegin").IsGreaterOrEqual(0);
            Condition.Requires(removeFromEnd, "removeFromEnd").IsGreaterOrEqual(0);
            Condition.Requires(value.Length, "value.Length").IsGreaterOrEqual(removeFromBegin + removeFromEnd);

            if (value.Length == removeFromBegin + removeFromEnd)
            {
                return string.Empty;
            }

            return value.Substring(removeFromBegin, value.Length - removeFromEnd - removeFromBegin);
        }

        public static string WrapWith(this string value, string wrapper)
        {
            return WrapWith(value, wrapper, wrapper);
        }

        public static string WrapWith(this string value, string start, string stop)
        {
            return (start ?? string.Empty) + value + (stop ?? string.Empty);
        }
        
        public static string WrapWith(this string value, char wrapper)
        {
            return WrapWith(value, wrapper, wrapper);
        }

        public static string WrapWith(this string value, char start, char stop)
        {
            return start + value + stop;
        }

        public static string EnsureEndsWith(this string aString, string expectedEnds)
        {
            if (string.IsNullOrEmpty(aString))
                throw new ArgumentNullException("aString");
            if (string.IsNullOrEmpty(expectedEnds))
                throw new ArgumentNullException("expectedEnds");

            if (aString.EndsWith(expectedEnds))
            {
                return aString;
            }
            return aString + expectedEnds;
        }

        public static StringBuilder EnsureEndsWith(this StringBuilder stringBuilder, string expectedEnds)
        {
            if (stringBuilder == null)
                throw new ArgumentNullException("stringBuilder");
            if (string.IsNullOrEmpty(expectedEnds))
                throw new ArgumentNullException("expectedEnds");

            if (stringBuilder.Length == 0 || stringBuilder.Length < expectedEnds.Length)
            {
                stringBuilder.Append(expectedEnds);
            }
            else
            {
                var endString = stringBuilder.ToString(stringBuilder.Length - expectedEnds.Length, expectedEnds.Length);
                if (!endString.Equals(expectedEnds,StringComparison.OrdinalIgnoreCase))
                {
                    stringBuilder.Append(expectedEnds);
                }
            }
            return stringBuilder;
        }

        public static string FormatThis(this string format, params object [] parameters)
        {
            if (format == null) 
                throw new ArgumentNullException("format");

            return string.Format(format, parameters);
        }

        public static string ReduceToStringWithLength(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

           if (source.Length < length)
           {
               length = source.Length;
           }

            return source.Substring(0, length);
        }

        public static string ReturnDefaultValueIfEmpty(this string source, string defaultValue)
        {
            return string.IsNullOrEmpty(source) ? defaultValue : source;
        }

        public static T ToEnum<T>(this string source)
            where T : struct
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            object value = Enum.Parse(typeof(T), source.Trim(), true);
            return (T) value;
        }

        public static T ToEnum<T>(this string source, T defaultValue)
            where T : struct
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;

            object value = Enum.Parse(typeof(T), source.Trim(), true);
            return (T) value;
        }

        public static string Transform(this string source, Func<string, string> transformFunc)
        {
            if (source == null) 
                throw new ArgumentNullException("source");
            if (transformFunc == null) 
                throw new ArgumentNullException("transformFunc");

            return transformFunc(source);
        }

        public static string HalfString(this string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source == string.Empty ? string.Empty : source.Substring(0, source.Length / 2);
        }

        public static Regex ToRegex(this string pattern)
        {
            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static string JoinToLines(this IEnumerable<string> strings)
        {
            var stringsArray = strings is string[] ? (string[]) strings : strings.ToArray();
            return string.Join(Environment.NewLine, stringsArray);
        }

        public static T SafeTo<T>(this string value)
        {
            return SafeTo(value, default(T));
        }

        public static T SafeTo<T>(this string value, T defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                return (T) Convert.ChangeType(value, typeof (T));
            }
            catch (FormatException)
            {
                return defaultValue;
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
            catch (OverflowException)
            {
                return defaultValue;
            }
        }
    }
}