using System;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumMapBuilder<TEnum>
        where TEnum : struct
    {
        private IEnumMap _resultEnumMap;

        internal IEnumMap ResultEnumMap
        {
            get { return _resultEnumMap; }
        }

        public void FromFile()
        {
            _resultEnumMap = null;
            throw new NotImplementedException();
        }

        public void ByEnumMap<TMapToEntity>(EnumMap<TEnum, TMapToEntity> enumMap)
            where TMapToEntity : ICacheableEntity
        {
            Condition.Requires(enumMap, "enumMap").IsNotNull();
            _resultEnumMap = enumMap;
        }

        public EnumMap<TEnum, TMapToEntity> MapEnumFrom<TMapToEntity>()
            where TMapToEntity : ICacheableEntity
        {
            var enumMap = new EnumMap<TEnum, TMapToEntity>();
            _resultEnumMap = enumMap;
            return enumMap;
        }
    }
}