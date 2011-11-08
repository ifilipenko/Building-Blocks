using System;
using System.Linq;
using System.Reflection;

namespace BuildingBlocks.Persistence.Conventions
{
    public class DefaultEntityCodeConvention : IEntityCodeConvention
    {
        public PropertyInfo ApplyTo(Type type)
        {
            return type.GetProperties().FirstOrDefault(IsCodeProperty);
        }

        public IConvention Clone()
        {
            return new DefaultEntityTitleConvention();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        private static bool IsCodeProperty(PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name.ToLower();
            return propertyName == "code" ||
                   propertyName == propertyInfo.DeclaringType.Name.ToLower() + "code";
        }
    }
}