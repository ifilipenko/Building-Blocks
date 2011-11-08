using System;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Utils
{
    public static class AttributesHelper
    {
        public static bool HasAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return HasAttribute(type, typeof(TAttribute));
        }

        public static bool HasAttribute(this Type type, Type attributeType)
        {
            Condition.Requires(type, "type").IsNotNull();
            Condition.Requires(attributeType, "attributeType").IsNotNull();
            return type.GetCustomAttributes(attributeType, true).Length > 0;
        }

        public static TAttribute GetAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            Condition.Requires(type, "type").IsNotNull();
            return (TAttribute)type.GetCustomAttributes(typeof(TAttribute), true).LastOrDefault();
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member)
            where TAttribute : Attribute
        {
            Condition.Requires(member, "member").IsNotNull();
            return (TAttribute) member.GetCustomAttributes(typeof (TAttribute), true).LastOrDefault();
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo member)
            where TAttribute : Attribute
        {
            Condition.Requires(member, "member").IsNotNull();
            return member.GetCustomAttributes(typeof (TAttribute), true).Length > 0;
        }
    }
}