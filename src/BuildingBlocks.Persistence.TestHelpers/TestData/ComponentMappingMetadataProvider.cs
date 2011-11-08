using NHibernate;
using NHibernate.Type;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    class ComponentMappingMetadataProvider : IMappingMetadataProvider
    {
        private readonly ComponentType _componentType;

        public ComponentMappingMetadataProvider(ComponentType componentType)
        {
            _componentType = componentType;
        }

        public object[] GetPropertyValues(object obj)
        {
            return _componentType.GetPropertyValues(obj, EntityMode.Poco);
        }

        public string[] GetPropertyNames()
        {
            return _componentType.PropertyNames;
        }

        public IType[] GetPropertyTypes()
        {
            return _componentType.Subtypes;
        }
    }
}