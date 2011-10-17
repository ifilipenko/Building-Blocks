using System;
using System.Collections.Generic;

namespace BuildingBlocks.Common.Sugar
{
    public static class ObjectExtentions
    {
        public static IEnumerable<TInstance> ToEnumerable<TInstance>(this TInstance instance)
        {
            yield return instance;
        }

        public static bool IsSame<TInstance>(this TInstance instance, TInstance other)
            where TInstance : class
        {
            return ReferenceEquals(instance, other);
        }

        public static bool IsNotSame<TInstance>(this TInstance instance, TInstance other)
            where TInstance : class
        {
            return !ReferenceEquals(instance, other);
        }

        public static bool WhenIsOf<T>(this object instance, Action<T> action)
        {
            if (action == null) 
                throw new ArgumentNullException("action");

            if (instance is T)
            {
                action((T) instance);
                return true;
            }
            return false;
        }

        public static bool IsOf<T>(this object instance)
        {
            return instance is T;
        }

        public static T CastTo<T>(this object instance)
        {
            return (T) instance;
        }

        public static T As<T>(this object instance) 
            where T : class
        {
            return instance as T;
        }

        public static T ForcedCastTo<T>(this object instance)
        {
            try
            {
                return (T)instance;
            }
            catch (Exception)
            {
                return (T) Convert.ChangeType(instance, typeof (T));
            }
        }

        public static string ConvertToString(this object @object)
        {
            if (ReferenceEquals(@object, null))
                return null;
            if (@object is string)
                return (string)@object;
            return @object.ToString();
        }
    }
}