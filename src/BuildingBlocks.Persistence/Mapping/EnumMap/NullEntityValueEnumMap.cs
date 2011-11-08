using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal class NullEntityValueEnumMap<TEnum, TProperty> : EnumValueMap<TEnum, TProperty> 
        where TEnum : struct
    {
        public NullEntityValueEnumMap(
            EnumValuesMap<TEnum, TProperty> valuesMap,
            IDictionary<EnumEntityValue, object> map)
            :base(default(TProperty), valuesMap, map)
        {
        }

        public override EnumValuesMap<TEnum, TProperty> Enum(TEnum enumValue)
        {
            return AddEnumValueMapToPropertyValue(null, enumValue);
        }
    }
}