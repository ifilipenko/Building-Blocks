using NHibernate.Type;

namespace BuildingBlocks.Persistence.TestHelpers.TestData
{
    interface IMappingMetadataProvider
    {
        object[] GetPropertyValues(object obj);
        string[] GetPropertyNames();
        IType[] GetPropertyTypes();
    }
}