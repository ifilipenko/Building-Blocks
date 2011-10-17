using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Reflection
{
    public static class TypeHelper
    {
        public static ConstructorInfo GetParameterLessConstructor(this Type type)
        {
            const BindingFlags onlyPublicConstructor = BindingFlags.Instance | BindingFlags.Public;
            var constructor = type.GetConstructor(
                onlyPublicConstructor,
                null,
                Type.EmptyTypes,
                null);
            return constructor;
        }

        public static bool HasParameterlessConstructor(this Type type)
        {
            return type.GetParameterLessConstructor() != null;
        }

        public static Type GetGenericAgrumentType(this Type type)
        {
            if (!type.IsGenericType && !type.BaseType.IsGenericType)
                throw new ArgumentException("Type is not generic", "type");

            return type.IsGenericType
                       ? type.GetGenericArguments().FirstOrDefault()
                       : type.BaseType.GetGenericArguments().FirstOrDefault();
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        public static Type GetElementType(Type enumerableType)
        {
            Type type = FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type != null)
            {
                return type.GetGenericArguments()[0];
            }
            return enumerableType;
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (type.IsNullableType())
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static bool IsEnumerableType(Type enumerableType)
        {
            return (FindGenericType(typeof(IEnumerable<>), enumerableType) != null);
        }

        public static bool IsKindOfGeneric(Type type, Type definition)
        {
            return (FindGenericType(definition, type) != null);
        }

        public static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
    }
}