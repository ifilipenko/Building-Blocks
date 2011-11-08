using System;

namespace BuildingBlocks.Common.Utils
{
    public static class SafeNullReferenceSyntax
    {
        public static TResult SafeGet<TInstance, TResult>(this TInstance instance, Func<TInstance, TResult> getter)
            where TInstance : class
        {
            return SafeGet(instance, getter, default(TResult));
        }

        public static TResult SafeGet<TInstance, TResult>(this TInstance? instance, Func<TInstance, TResult> getter, TResult defaultValue = default(TResult))
            where TInstance : struct 
        {
            if (getter == null)
                throw new ArgumentNullException("getter");
            if (instance == null)
                return defaultValue;

            return getter(instance.Value);
        }

        public static TResult SafeGet<TInstance, TResult>(this TInstance instance, Func<TInstance, TResult> getter, TResult defaultValue)
            where TInstance : class
        {
            if (getter == null)
                throw new ArgumentNullException("getter");

            if (instance == null)
                return defaultValue;

            try
            {
                var result = getter(instance);
                return ReferenceEquals(result, null) ? defaultValue : result;
            }
            catch (NullReferenceException)
            {
                return defaultValue;
            }
            catch (ArgumentNullException)
            {
                return defaultValue;
            }
        }

        public static void SafeDo<TInstance>(this TInstance instance, Action<TInstance> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            if (ReferenceEquals(instance, null))
                return;

            action(instance);
        }

        public static void SafeSet<TInstance>(this TInstance instance, Action<TInstance> setter)
            where TInstance : class
        {
            if (setter == null)
                throw new ArgumentNullException("setter");

            if (instance == null)
                return;

            try
            {
                setter(instance);
            }
            catch (NullReferenceException)
            {
            }
        }
    }
}