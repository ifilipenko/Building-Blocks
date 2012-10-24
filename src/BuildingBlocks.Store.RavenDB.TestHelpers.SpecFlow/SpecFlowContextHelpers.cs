using System;
using System.Linq.Expressions;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow
{
    public static class SpecFlowContextHelpers
    {
        public static T Obtain<T>(this SpecFlowContext context, string key = null)
            where T : new()
        {
            T instance;
            return context.TryGetValue(key, out instance) ? instance : new T();
        }

        public static T Obtain<T>(this SpecFlowContext context, Func<T> factory, string key = null)
        {
            T instance;
            if (!TryGetInstance(context, key, out instance))
            {
                instance = factory();
                if (key == null)
                {
                    context.Set(instance);
                }
                else
                {
                    context.Set(instance, key);
                }
            }
            return instance;
        }

        public static T Obtain<T>(this SpecFlowContext context, Func<T> factory, Expression<Func<T>> property)
            where T : class
        {
            var member = ((MemberExpression) property.Body).Member;
            return Obtain(context, factory, member.Name);
        }

        private static bool TryGetInstance<T>(SpecFlowContext context, string key, out T instance)
        {
            instance = default(T);
            try
            {
                instance = key == null
                               ? context.Get<T>()
                               : context.Get<T>(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}