using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace BuildingBlocks.Common.Configuration
{
    public class KeyValueConfiguration : IConfiguration
    {
        private readonly Func<string, string> _configItemReader;

        public KeyValueConfiguration(Func<string, string> configItemReader)
        {
            if (configItemReader == null)
                throw new ArgumentNullException("configItemReader");
            _configItemReader = configItemReader;
        }

        public KeyValueConfiguration(Func<NameValueCollection> configurationItemSource)
            : this(x => ReadFromNameValueCollection(configurationItemSource(), x))
        {
            if (configurationItemSource == null)
                throw new ArgumentNullException("configurationItemSource");
        }

        public KeyValueConfiguration(Func<IDictionary> configurationItemSource)
            : this(x => ReadFromDictionary(configurationItemSource(), x))
        {
            if (configurationItemSource == null)
                throw new ArgumentNullException("configurationItemSource");
        }

        public string GetSetting(string name)
        {
            return _configItemReader(name);
        }

        public T LoadTo<T>()
            where T : new()
        {
            var result = new T();
            foreach (var property in typeof(T).GetProperties())
            {
                var value = GetValueForProperty(property);
                if (value != null)
                {
                    property.SetValue(result, value, null);
                }
            }
            return result;
        }

        private object GetValueForProperty(PropertyInfo property)
        {
            switch (Type.GetTypeCode(property.PropertyType))
            {
                case TypeCode.Boolean:
                    return Boolean.Parse(GetSetting(property.Name));
                case TypeCode.Char:
                    return Char.Parse(GetSetting(property.Name));
                case TypeCode.SByte:
                    return SByte.Parse(GetSetting(property.Name));
                case TypeCode.Byte:
                    return Byte.Parse(GetSetting(property.Name));
                case TypeCode.Int16:
                    return Int16.Parse(GetSetting(property.Name));
                case TypeCode.UInt16:
                    return UInt16.Parse(GetSetting(property.Name));
                case TypeCode.Int32:
                    return Int32.Parse(GetSetting(property.Name));
                case TypeCode.UInt32:
                    return UInt32.Parse(GetSetting(property.Name));
                case TypeCode.Int64:
                    return Int64.Parse(GetSetting(property.Name));
                case TypeCode.UInt64:
                    return UInt64.Parse(GetSetting(property.Name));
                case TypeCode.Single:
                    return Single.Parse(GetSetting(property.Name));
                case TypeCode.Double:
                    return Double.Parse(GetSetting(property.Name));
                case TypeCode.Decimal:
                    return Decimal.Parse(GetSetting(property.Name));
                case TypeCode.DateTime:
                    return DateTime.Parse(GetSetting(property.Name));
                case TypeCode.String:
                    return GetSetting(property.Name);
            }
            return null;
        }

        private static string ReadFromNameValueCollection(NameValueCollection contigurationItemSource, string key)
        {
            return contigurationItemSource[key];
        }

        private static string ReadFromDictionary(IDictionary contigurationItemSource, string key)
        {
            var result = contigurationItemSource[key];
            if (result == null || result is string)
                return (string) result;
            return result.ToString();
        }
    }
}
