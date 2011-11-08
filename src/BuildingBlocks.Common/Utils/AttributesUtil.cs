using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingBlocks.Common.Utils
{
    public static class AttributesUtil
    {
        public static IDictionary<MethodInfo, TAttribute> FindMethodsWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            return _FindMethodsWithAttribute<TAttribute>(type, AttributeSearchAreas.All);
        }

        public static IDictionary<MethodInfo, TAttribute> FindMethodsWithAttribute<TAttribute>(Type type,
            AttributeSearchAreas searchAreas)
            where TAttribute : Attribute
        {
            return _FindMethodsWithAttribute<TAttribute>(type, searchAreas);
        }

        public static bool MethodAttributeAreInherited<TAttribute>(Type type, string methodName)
            where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        private static IDictionary<MethodInfo, TAttribute> _FindMethodsWithAttribute<TAttribute>(Type type,
            AttributeSearchAreas searchAreas)
        {
            bool inherited = (searchAreas & AttributeSearchAreas.BaseClasses) == AttributeSearchAreas.BaseClasses;
            bool searchInInterfaces = (searchAreas & AttributeSearchAreas.Interfaces) == AttributeSearchAreas.Interfaces;

            Dictionary<MethodInfo, TAttribute> methods = new Dictionary<MethodInfo, TAttribute>();
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] attributes = method.GetCustomAttributes(typeof(TAttribute), inherited);
                if (attributes != null && attributes.Length > 0)
                {
                    methods.Add(method, (TAttribute)attributes[0]);
                }
            }

            if (searchInInterfaces)
            {
                AddInterfacesAttributes(type, inherited, methods);
            }

            return methods;
        }

        private static void AddInterfacesAttributes<TAttribute>(Type type, bool inherited, 
            IDictionary<MethodInfo, TAttribute> methods)
        {
            foreach (Type intrefaceType in type.GetInterfaces())
            {
                IDictionary<MethodInfo, TAttribute> fromInterfaces =
                    _FindMethodsWithAttribute<TAttribute>(intrefaceType, AttributeSearchAreas.TargetClass);

                if (fromInterfaces.Count > 0)
                {
                    foreach (KeyValuePair<MethodInfo, TAttribute> pair in fromInterfaces)
                    {
                        if (methods.ContainsKey(pair.Key))
                        {
                            continue;
                        }

                        methods.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        public static TAttribute FindClassAttribute<TAttribute>(Type type)
        {
            return FindClassAttribute<TAttribute>(type, AttributeSearchAreas.All);
        }

        public static TAttribute FindClassAttribute<TAttribute>(Type type, AttributeSearchAreas searchAreas)
        {
            bool inherited = (searchAreas & AttributeSearchAreas.BaseClasses) == AttributeSearchAreas.BaseClasses;
            bool searchInInterfaces = (searchAreas & AttributeSearchAreas.Interfaces) == AttributeSearchAreas.Interfaces;

            object[] attributes = type.GetCustomAttributes(typeof(TAttribute), inherited);
            if (attributes == null || attributes.Length == 0)
            {
                if (searchInInterfaces)
                {
                    foreach (Type intrefaceType in type.GetInterfaces())
                    {
                        attributes = intrefaceType.GetCustomAttributes(typeof (TAttribute), inherited);
                        if (attributes.Length != 0)
                        {
                            return (TAttribute)attributes[0];
                        }
                    }
                }
                return default(TAttribute);
            }

            return (TAttribute) attributes[0];
        }

        public static T GetPropertyAttribute<T>(Type type, string property)
            where T : Attribute
        {
            PropertyInfo propertyInfo = type.GetProperty(property);
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Свойство [{0}] отсутствует в классе [{1}]", property, type),
                                            "property");
            }

            object[] attributes = propertyInfo.GetCustomAttributes(typeof (T), true);
            if (attributes == null || attributes.Length == 0)
                return null;

            return (T) attributes[attributes.Length - 1];
        }

        public static T GetPropertyAttribute<T>(PropertyInfo property)
            where T : Attribute
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            object[] attributes = property.GetCustomAttributes(typeof(T), true);
            if (attributes == null || attributes.Length == 0)
                return null;

            return (T) attributes[attributes.Length - 1];
        }
    }
}