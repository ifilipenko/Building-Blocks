using FluentNHibernate.Mapping;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumMapPropertyPart
    {
        private readonly PropertyPart _propertyPart;

        public EnumMapPropertyPart(PropertyPart propertyPart)
        {
            _propertyPart = propertyPart;
        }

        public EnumMapPropertyPart ReadOnly()
        {
            _propertyPart.ReadOnly();
            return this;
        }
    }
}