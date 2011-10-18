using FluentNHibernate.Mapping;

namespace BuildingBlocks.Persistence.Mapping
{
    public static class EntityMapExtentions
    {
        public static OneToManyPart<T> MapAsAggregateItems<T>(this OneToManyPart<T> oneToManyPart, string keyColumn)
        {
            return oneToManyPart
                .Inverse()
                .KeyColumn(keyColumn)
                .LazyLoad()
                .AsBag()
                .Cascade.AllDeleteOrphan();
        }
    }
}