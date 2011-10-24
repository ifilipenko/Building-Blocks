using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class TariffPlanMap : ClassMap<TariffPlan>
    {
        public TariffPlanMap()
        {
            Table("TARIFF_PLAN");

            Id(s => s.TariffPlanID, "id");
            Map(s => s.Name, "name");
            HasMany(s => s.Versions) 
                .KeyColumns.Add("tariff_plan_id")
                .Fetch.Join()
                .Inverse()
                .Cascade.All();
        }
    }

    public class TariffPlanVersionMap : ClassMap<TariffPlanVersion>
    {
        public TariffPlanVersionMap()
        {
            Table("TARIFF_PLAN_VERSION");

            Id(s => s.TariffPlanVersionID, "id");
            Map(s => s.Version, "num");
            References(s => s.TariffPlan, "tariff_plan_id");
        }
    }

    public class ContractMap : ClassMap<Contract>
    {
        public ContractMap()
        {
            Table("contract");

            Id(t => t.ContractID, "id");
            HasManyToMany(c => c.TariffPlans)
                .Table("CONTRACT_TARIFF_PLAN")
                .AsBag()
                .Cascade.AllDeleteOrphan();
        }
    }
}