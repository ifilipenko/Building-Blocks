using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class ServiceMap : ClassMap<Service>
    {
        public ServiceMap()
        {
            Id(s => s.ID);
            Map(s => s.Quantity);
            References(s => s.ServiceType);
            References(s => s.Case);
        }
    }

    public class PatientCaseMap : ClassMap<PatientCase>
    {
        public PatientCaseMap()
        {
            Id(s => s.ID);
            Map(s => s.PatientName);
        }
    }

    public class CountryMap : ClassMap<Country>
    {
        public CountryMap()
        {
            Id(c => c.ID);
            Map(c => c.Name);
        }
    }

    public class ServiceTypeMap : ClassMap<ServiceType>
    {
        public ServiceTypeMap()
        {
            Id(s => s.ID);
            Map(s => s.LocalCode).Not.Nullable();
            Map(s => s.FederalCode).Not.Nullable();
            Map(s => s.Name).Not.Nullable();
            Map(s => s.Number).Not.Nullable();
            Component(s => s.Currency, MapCurrency);
        }

        private static void MapCurrency(ComponentPart<Currency> componentPart)
        {
            componentPart.Map(c => c.FullName);
            componentPart.Map(c => c.ShortName);
            componentPart.Map(c => c.Sign);
            componentPart.References(c => c.Country).Not.Nullable();
        }
    }
}