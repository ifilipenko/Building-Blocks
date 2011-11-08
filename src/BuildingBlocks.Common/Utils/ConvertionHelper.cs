using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BuildingBlocks.Common.Utils
{
    public static class ConvertionHelper
    {
        public static string ClassAsString<T>(this T nullableValue)
            where T : class
        {
            return ClassAsString(nullableValue, string.Empty);
        }

        public static string ClassAsString<T>(this T nullableValue, string defaultString)
            where T : class
        {
            return ClassAsString(nullableValue, string.Empty, defaultString);
        }

        public static string ClassAsString<T>(this T nullableValue, string pattern, string defaultString)
            where T : class
        {
            if (nullableValue == null)
                return defaultString;

            return string.IsNullOrEmpty(pattern)
                       ? nullableValue.ToString()
                       : string.Format("{0:" + pattern + "}", nullableValue);
        }

        public static string NullableAsString<T>(this T? nullableValue)
            where T : struct
        {
            return NullableAsString(nullableValue, string.Empty);
        }

        public static string NullableAsString<T>(this T? nullableValue, string defaultString)
            where T : struct
        {
            return NullableAsString(nullableValue, string.Empty, defaultString);
        }

        public static string NullableAsString<T>(this T? nullableValue, string pattern, string defaultString)
            where T : struct
        {
            if (nullableValue.HasValue)
            {
                return string.IsNullOrEmpty(pattern)
                           ? nullableValue.Value.ToString()
                           : string.Format("{0:" + pattern + "}", nullableValue.Value);
            }

            return defaultString;
        }

        public static bool PropertiesCanCompare(PropertyInfo property1, PropertyInfo property2)
        {
            Debug.Assert(property1 != null);
            Debug.Assert(property2 != null);

            return
                Equals(property1, property2) ||
                (property1.PropertyType == property2.PropertyType) ||
                (property1.PropertyType.IsAssignableFrom(property2.PropertyType) ||
                 property2.PropertyType.IsAssignableFrom(property1.PropertyType));
        }

        public static bool CanCompareWithType(object value, Type typeToComapare)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType().IsAssignableFrom(typeToComapare) ||
                typeToComapare.IsAssignableFrom(value.GetType()))
            {
                return true;
            }

            try
            {
                Convert.ChangeType(value, typeToComapare);
                return true;
            }
            catch(InvalidCastException)
            {
                return false;
            }
        }

        public static bool PropertiesCanCompare(IList<PropertyInfo> left, IList<PropertyInfo> right)
        {
            PropertyInfo leftLast = left[left.Count - 1];
            PropertyInfo rightLast = right[left.Count - 1];
            return PropertiesCanCompare(leftLast, rightLast);
        }
    }
}