using System;
using System.Collections.Generic;
using System.Configuration;

namespace BuildingBlocks.Configuration.Utils
{
    public static class ConfigurationManagerHelper
    {
        public static void LoadSettingTo<TValue>(string settingKey, Action<TValue> settingDestination, bool requared = false)
        {
            var value = ConfigurationManager.AppSettings[settingKey];
            if (string.IsNullOrEmpty(value))
            {
                if (requared)
                    throw new ConfigurationErrorsException("Setting with key \"" + settingKey + "\" is not found");
                return;
            }

            TValue convertedValue;
            try
            {
                convertedValue = (TValue) Convert.ChangeType(value, typeof(TValue));
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Setting with key \"" + settingKey + "\" has invalid invalid format", ex);
            }
            settingDestination(convertedValue);
        }

        public static void LoadSettingTo(string settingKey, Action<string> settingDestination, bool requared = false)
        {
            var value = ConfigurationManager.AppSettings[settingKey];
            if (string.IsNullOrEmpty(value))
            {
                if (requared)
                    throw new ConfigurationErrorsException("Setting with key \"" + settingKey + "\" is not found");
                return;
            }

            settingDestination(value);
        }

        public static IDictionary<string ,string> GetAllSettings()
        {
            var result = new Dictionary<string, string>();

            for (int index = 0; index < ConfigurationManager.AppSettings.AllKeys.Length; index++)
            {
                var key = ConfigurationManager.AppSettings.AllKeys[index];
                var value = ConfigurationManager.AppSettings[index];
                result.Add(key, value);
            }
            
            return result;
        }
    }
}
