using System;
using System.Reflection;

namespace BuildingBlocks.Reflection
{
    public class Class : IEquatable<Class>
    {
        public static T CreateInstanceFor<T>()
            where T : class
        {
            Class clazz = new Class(typeof(T));
            return (T) clazz.InvokeDefaultConstructor();
        }

        private readonly Type _classType;

        public Class(Type classType)
        {
            if (!classType.IsClass && !classType.IsInterface)
            {
                throw new ArgumentException(string.Format("Type \"{0}\" is not class or interface", ClassType));
            }
            _classType = classType;
        }

        public Type ClassType
        {
            get { return _classType; }
        }

        public PropertyInfo GetProperty(string property)
        {
            PropertyInfo result = ClassType.GetProperty(property, GetPropertysBindingFlags());
            if (result == null)
            {
                throw new ArgumentException(
                    string.Format("Property named \"{0}\" not contain in class \"{1}\"", property, ClassType), "property");
            }
            return result;
        }

        public PropertyInfo GetProperty(string property, Type propertyType)
        {
            PropertyInfo[] result = ClassType.GetProperties(GetPropertysBindingFlags());
            foreach (PropertyInfo propertyInfo in result)
            {
                if (propertyInfo.Name == property && propertyInfo.PropertyType == propertyType)
                {
                    return propertyInfo;
                }
            }

            throw new ArgumentException(
                string.Format("Property named \"{0}\" not contain in class \"{1}\"", property, ClassType), "property");
        }

        private static BindingFlags GetPropertysBindingFlags()
        {
            return BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty |
                   BindingFlags.Instance;
        }

        public Class GetPropertyClass(string property)
        {
            PropertyInfo propertyInfo = GetProperty(property);
            return new Class(propertyInfo.PropertyType);
        }

        public void ValidatePropertyName(string property)
        {
            GetProperty(property);
        }

        public ConstructorInfo GetDefaultConstructor()
        {
            return _classType.GetConstructor(Type.EmptyTypes);
        }

        public object InvokeConstructorInstance(bool invokeNonPublicConstructor, Type[] paramTypes, object[] paramValues)
        {
            const BindingFlags onlyPublicConstructor = BindingFlags.Instance | BindingFlags.Public;
            const BindingFlags anyConstructor = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ConstructorInfo constructor = ClassType.GetConstructor(
                invokeNonPublicConstructor ? anyConstructor : onlyPublicConstructor,
                null,
                paramTypes,
                null);
            return constructor == null ? null : constructor.Invoke(paramValues);
        }

        public object InvokeDefaultConstructor()
        {
            return InvokeConstructorInstance(true, Type.EmptyTypes, null);
        }

        #region [Overrides object methods]

        public override string ToString()
        {
            return _classType.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Class)) return false;
            return Equals((Class)obj);
        }

        public bool Equals(Class obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._classType, _classType);
        }

        public override int GetHashCode()
        {
            return (_classType != null ? _classType.GetHashCode() : 0);
        }

        public static bool operator ==(Class left, Class right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Class left, Class right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}