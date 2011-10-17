using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.Sugar;
using BuildingBlocks.Common.Utils;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Reflection
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetBaseTypesList(this Type type)
        {
            Condition.Requires(type, "type").IsNotNull();

            var types = new List<Type>();
            while (type.BaseType != null)
            {
                types.Add(type.BaseType);
                type = type.BaseType;
            }
            return types;
        }

        public static bool InheritedFrom(this Type type, Type expectedBaseType)
        {
            Condition.Requires(type, "type").IsNotNull();
            Condition.Requires(expectedBaseType, "expectedBaseType").IsNotNull();

            while (type != null)
            {
                if (type.BaseType == expectedBaseType)
                {
                    return true;
                }
                type = type.BaseType;
            }

            return false;
        }

        public static bool InheritedFrom<ExpectedBaseType>(this Type type)
        {
            return InheritedFrom(type, typeof (ExpectedBaseType));
        }

        public static IDictionary<string, object> GetPropertiesValues(this object instance)
        {
            if (instance == null) 
                throw new ArgumentNullException("instance");

            var type = instance.GetType();
            var properties = type.GetProperties();

            var dictionary = new Dictionary<string, object>(properties.Length);
            foreach (var property in properties)
            {
                var value = property.GetValue(instance, null);
                dictionary.Add(property.Name, value);
            }
            return dictionary;
        }

        public static Pair<TEnum, Attribute[]>[] GetEnumValuesAttributes<TEnum>()
            where TEnum : struct 
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Expected enum type? but was " + enumType);
            
            var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);
            var result = new Pair<TEnum, Attribute[]>[fields.Length];
            for (int index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var value = (TEnum) field.GetValue(null);
                var attributes = field.GetCustomAttributes(true).OfType<Attribute>().ToArray();
                result[index] = new Pair<TEnum, Attribute[]>(value, attributes);
            }

            return result;
        }

        public static FieldInfo GetEnumValueInfo<TEnum>(this TEnum enumValue)
            where TEnum : struct
        {
            var enumElements = GetEnumElementInfos<TEnum>();
            return enumElements.First(t => t.Name == enumValue.ToString());
        }

        public static IEnumerable<FieldInfo> GetEnumElementInfos<TEnum>()
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Expected enum type? but was " + enumType);

            return enumType
                .GetMembers()
                .OfType<FieldInfo>()
                .Where(fi => !fi.IsSpecialName);
        }

        public static void InvokePrivateMethod(this object instance, string methodName, params object[] parameters)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException("methodName");

            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            parameters = parameters ?? new object[0];
            var method = methods.FirstOrDefault(m => m.Name == methodName &&
                                                     ParametersValuesIsValid(m, parameters));

            if (method == null)
            {
                throw new InvalidOperationException("Method with given name not exists in type " + instance.GetType());
            }

            method.Invoke(instance, parameters);
        }

        public static TResult InvokePrivateMethod<TResult>(this object instance, string methodName, params object[] parameters)
        {
            if (instance == null) 
                throw new ArgumentNullException("instance");
            if (string.IsNullOrEmpty(methodName)) 
                throw new ArgumentNullException("methodName");

            var methods = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            parameters = parameters ?? new object[0];
            var method = methods.FirstOrDefault(m => m.Name == methodName &&
                                                     typeof (TResult).IsAssignableFrom(m.ReturnType) &&
                                                     ParametersValuesIsValid(m, parameters));

            if (method == null)
            {
                throw new InvalidOperationException("Method with given name not exists in type " + instance.GetType());
            }

            object result = method.Invoke(instance, parameters);
            return (TResult) result;
        }

        private static bool ParametersValuesIsValid(MethodInfo methodInfo, object[] parameters)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != parameters.Length)
                return false;

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                object paramValue = parameters[parameterInfo.Position];
                if (paramValue == null)
                {
                    if (parameterInfo.ParameterType.IsValueType)
                        return false;
                }
                else if (!parameterInfo.ParameterType.IsAssignableFrom(paramValue.GetType()))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValueCanConvertToString(this object value)
        {
            if (value == null)
                return false;
            Type valueType = value.GetType();
            TypeCode typeCode = Type.GetTypeCode(valueType);
            return typeCode != TypeCode.Object && typeCode != TypeCode.Empty;
        }

        public static PropertyInfo[] GetPropertyPath<T>(this string propertyNamesPath)
        {
            return GetPropertyPath(propertyNamesPath, typeof(T));
        }

        public static PropertyInfo[] GetPropertyPath(this string propertyNamesPath, Type type)
        {
            Condition.Requires(type, "type").IsNotNull();

            if (string.IsNullOrEmpty(propertyNamesPath))
                throw new ArgumentNullException("propertyNamesPath");

            var namesPath = propertyNamesPath.Split(".".ToCharArray());

            var path = new PropertyInfo[namesPath.Length];
            for (int i = 0; i < namesPath.Length; i++)
            {
                var propertyName = namesPath[i];
                if (string.IsNullOrEmpty(propertyName))
                    throw new ArgumentException("Invalid property path " + propertyNamesPath, "propertyNamesPath");

                type = i == 0 ? type : path[i - 1].PropertyType;
                var property = propertyName.ToPropertyInfoOf(type);

                if (property == null)
                {
                    var message = string.Format("Invalid property path [{0}]: property [{1}] not exists in [{2}]",
                                                   propertyNamesPath, propertyName, type.FullName);
                    throw new ArgumentException(message, "propertyNamesPath");
                }

                path[i] = property;
            }

            return path;
        }

        public static void SetMember<T, TProp>(this T instance, Expression<Func<T, TProp>> memberGetter, TProp value)
        {
            var memberInfo = memberGetter.GetMemberInfo();
            SetMember(instance, value, memberInfo);
        }

        public static void SetMember<T>(this T instance, string memberName, object value)
        {
            var members = typeof (T).GetMember(memberName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.SetProperty);
            if (members.Length == 0)
                throw new InvalidOperationException("Not found modifiable field or property with name " + memberName + " in type " + typeof(T));
            foreach (var member in members)
            {
                SetMember(instance, value, member);
            }
        }

        private static void SetMember<T, TProp>(T instance, TProp value, MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
                memberInfo.CastTo<PropertyInfo>().SetValue(instance, value, null);
            else if (memberInfo is FieldInfo)
                memberInfo.CastTo<FieldInfo>().SetValue(instance, value);
            else
                throw new InvalidOperationException("Member is not field or property");
        }

        public static PropertyInfo ToPropertyInfoOf<T>(this string propertyName)
        {
            return ToPropertyInfoOf(propertyName, typeof(T));
        }

        public static PropertyInfo ToPropertyInfoOf(this string propertyName, Type type)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;
            return type.GetProperty(propertyName);
        }

        public static FieldInfo GetField(this object instance, string fieldName)
        {
            Condition.Requires(fieldName, "fieldName").IsNotNullOrEmpty();
            Condition.Requires(instance, "instance").IsNotNull();
            var field = instance.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.GetField | BindingFlags.SetField |
                BindingFlags.Instance | BindingFlags.IgnoreCase);
            return field;
        }

        public static object GetFieldValue(this object instance, string fieldName)
        {
            var field = instance.GetField(fieldName);
            Condition.Ensures(field, "field").IsNotNull();
            return field.GetValue(instance);
        }

        public static T GetFieldValueAs<T>(this object instance, string fieldName)
        {
            return (T) GetFieldValue(instance, fieldName);
        }

        [Obsolete]
        public static PropertyInfo GetPropertyByName<T>(this string propertyName)
        {
            return ToPropertyInfoOf<T>(propertyName);
        }

        // todo: дать более точное и понятное название
        public static TValue ToPropertyValueOfInstance<TValue>(this string propertyName, object instance)
        {
            return GetPropertyValueOfInstance(propertyName, instance, default(TValue), false);
        }

        public static TValue ToPropertyValueOfInstance<TValue>(this string propertyName, object instance, TValue defaultValueIfPropertyNotExists)
        {
            return GetPropertyValueOfInstance(propertyName, instance, defaultValueIfPropertyNotExists, true);
        }

        private static TValue GetPropertyValueOfInstance<TValue>(this string propertyName, object instance, TValue defaultValueIfPropertyNotExists, bool ignoreErrors)
        {
            PropertyInfo propertyInfo = ToPropertyInfoOf(propertyName, instance.GetType());
            if (propertyInfo == null)
            {
                if (ignoreErrors)
                    return defaultValueIfPropertyNotExists;

                string message = string.Format("Property with given name \"{0}\" not exists in object of type \"{1}\"",
                                               propertyName, instance.GetType());
                throw new ArgumentException(message, "propertyName");
            }

            return (TValue)propertyInfo.GetValue(instance, null);
        }

        public static bool IsImplementInterface<TInterface>(this Type type)
        {
            return IsImplementedBy(typeof (TInterface), type);
        }

        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            return IsImplementedBy(interfaceType, type);
        }

        public static bool IsImplementedBy(this Type interfaceType, Type classType)
        {
            Debug.Assert(classType != null);
            Debug.Assert(interfaceType != null);
            Debug.Assert(interfaceType.IsInterface, "Тип параметра Interface должен быть интефейсом");

            if (interfaceType.IsGenericTypeDefinition)
            {
                return classType.GetInterfaces().Any(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == interfaceType);
            }
            return classType.GetInterfaces().Any(iface => iface == interfaceType);
        }

        [Obsolete("Use IsImplementInterface")]
        public static bool TypeIsImplementInterface<TInterface>(this Type type)
        {
            return IsImplementedBy(typeof(TInterface), type);
        }

        [Obsolete("Use ImplementedBy")]
        public static bool TypeIsImplementInterface(this Type interfaceType, Type classType)
        {
            return IsImplementedBy(interfaceType, classType);
        }
    }
}