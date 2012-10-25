using System;
using System.Collections.Specialized;
using System.Configuration;

namespace BuildingBlocks.Membership
{
    static class ConfigHelper
    {
        public static T GetOf<T>(this NameValueCollection config, string sectionName, T defaultValue = default(T), Func<T, string> validator = null)
        {
            string settingValue;
            try
            {
                settingValue = config[sectionName];
            }
            catch (Exception)
            {
                return defaultValue;
            }

            T value;
            try
            {
                value = (T)Convert.ChangeType(settingValue, typeof(T));
            }
            catch (Exception)
            {
                throw new InvalidCastException(string.Format("Value \"{0}\" of membership attribute \"{1}\" can be converted to \"{2}\"", settingValue, sectionName, typeof(T)));
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
    }
}