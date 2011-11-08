using FluentNHibernate.Cfg;

namespace BuildingBlocks.Persistence.Exports
{
    public interface IMappingExporter
    {
        void Export(FluentMappingsContainer fluentMappings);
        void Export(AutoMappingsContainer autoMappings);
        IMappingExporter Clone();
    }
}