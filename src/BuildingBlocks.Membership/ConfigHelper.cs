using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace BuildingBlocks.Membership
{
    public static class ConfigHelper
    {
        public static T GetOf<T>(this NameValueCollection config, string sectionName, T defaultValue = default(T), Func<T, string> validator = null)
        {
            if (config.AllKeys.All(v => v != sectionName))
                return defaultValue;

            string settingValue;
            try
            {
                settingValue = config[sectionName];
            }
            catch (Exception)
            {
                return defaultValue;
            }

            if (string.IsNullOrWhiteSpace(settingValue))
                return defaultValue;

            T value;
            try
            {
                if (IsNullable(typeof(T)))
                {
                    value = (T) Convert.ChangeType(settingValue, UnwrapNullableType(typeof (T)));
                }
                else
                {
                    value = (T) Convert.ChangeType(settingValue, typeof (T));
                }
            }
            catch (Exception)
            {
                var type = typeof (T);
                if (IsNullable(type))
                {
                    type = UnwrapNullableType(typeof (T));
                }
                throw new InvalidCastException(string.Format("Value \"{0}\" of membership attribute \"{1}\" can be converted to \"{2}\"", settingValue, sectionName, type));
            }

            if (validator != null)
            {
                var error = validator(value);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new ConfigurationErrorsException(string.Format("Value \"{0}\" of membership attribute \"{1}\" is invalid: \"{2}\"", value, sectionName, error));
                }
            }

            return value;
        }

        private static Type UnwrapNullableType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }
    }
}