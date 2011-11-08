using NHibernate;
using NHibernate.Metadata;
using NHibernate.Type;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    class EntityMappingMetadataProvider : IMappingMetadataProvider
    {
        private readonly IClassMetadata _classMetadata;

        public EntityMappingMetadataProvider(IClassMetadata classMetadata)
        {
            _classMetadata = classMetadata;
        }

        public object[] GetPropertyValues(object obj)
        {
            return _classMetadata.GetPropertyValues(obj, EntityMode.Poco);
        }

        public string[] GetPropertyNames()
        {
            return _classMetadata.PropertyNames;
        }

        public IType[] GetPropertyTypes()
        {
            return _classMetadata.PropertyTypes;
        }
    }
}