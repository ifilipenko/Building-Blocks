using System;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Persistence.Conventions
{
    public class DefaultEntityTitleConvention : IEntityTitleConvention
    {
        public PropertyInfo ApplyTo(Type type)
        {
            return type.GetProperties().FirstOrDefault(IsTitleProperty);
        }

        public IConvention Clone()
        {
            return new DefaultEntityTitleConvention();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        private static bool IsTitleProperty(PropertyInfo propertyInfo)
        {
            var hasTitleAttribute = propertyInfo.GetCustomAttributes(typeof(TitleAttribute), true).Length != 0;
            if (hasTitleAttribute)
                return true;
            if (propertyInfo.PropertyType != typeof(string))
                return false;
            var propertyName = propertyInfo.Name.ToLower();
            return propertyName == "name" ||
                   propertyName == "title" ||
                   propertyName == propertyInfo.DeclaringType.Name.ToLower() + "Name" ||
                   propertyName == propertyInfo.DeclaringType.Name.ToLower() + "title";
        }
    }
}