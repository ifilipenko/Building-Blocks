using System;
using System.Reflection;
using System.Text;

namespace BuildingBlocks.Common
{
    public class ReflectedToString
    {
        private readonly Type _classType;
        private readonly object _instance;

        public ReflectedToString(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentException("ReflectedToString requared non null parameters", "instance");
            }
            _classType = instance.GetType();
            _instance = instance;
        }

        public string GetString()
        {
            StringBuilder builder = new StringBuilder();
            PropertyInfo[] properties = _classType.GetProperties();

            builder.Append(_classType.Name + "{");
            bool first = true;
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!first)
                {
                    builder.Append("; ");
                }
                try
                {
                    builder.AppendFormat("{0}({1})=", propertyInfo.Name, propertyInfo.PropertyType);
                    object value = propertyInfo.GetValue(_instance, null);
                    if (value != null)
                    {
                        builder.Append(value);
                    }
                }
                catch
                {
                }
                first = false;
            }
            builder.Append('}');

            return builder.ToString();
        }
    }
}