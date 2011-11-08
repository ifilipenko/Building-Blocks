using BuildingBlocks.Testing.Persistence.Model;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Testing.Persistence.Maps
{
    public class AggregateEntitiesMaps : ClassMap<PreParent>
    {
        public AggregateEntitiesMaps()
        {
            Table("PreParent");

            Id(r => r.ID);
            HasMany(r => r.Childs)
                .AsBag()
                .Inverse()
                .Cascade.AllDeleteOrphan()
                .KeyColumns.Add("ParentID")
                .LazyLoad();
        }
    }

    public class AggregateChildMaps : ClassMap<Parent>
    {
        public AggregateChildMaps()
        {
            Table("Parent");

            Id(r => r.ID);
            References(c => c.PreParent, "ParentID")
                .Not.Nullable()
                .LazyLoad();
            HasMany(r => r.Childs)
                .AsBag()
                .Inverse()
                .Cascade.AllDeleteOrphan()
                .KeyColumns.Add("Parent2LID")
                .LazyLoad();
        }
    }

    public class AggregateChild2Maps : ClassMap<Child>
    {
        public AggregateChild2Maps()
        {
            Table("Child");

            Id(r => r.ID);
            References(c => c.Parent, "Parent2LID")
                .Not.Nullable()
                .LazyLoad();
        }
    }
}