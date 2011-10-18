using System;
using System.Reflection;

namespace BuildingBlocks.Persistence.Conventions
{
    class NullEntityTitleConvention : IEntityTitleConvention
    {
        public PropertyInfo ApplyTo(Type instance)
        {
            return null;
        }

        public bool IsTitleProperty(PropertyInfo propertyInfo)
        {
            return false;
        }

        public IConvention Clone()
        {
            return new NullEntityTitleConvention();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}