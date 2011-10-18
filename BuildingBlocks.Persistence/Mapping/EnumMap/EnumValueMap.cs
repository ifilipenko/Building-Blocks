using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumValueMap<TEnum, TProperty> 
        where TEnum : struct
    {
        private readonly TProperty _entityValue;
        private readonly EnumValuesMap<TEnum, TProperty> _valuesMap;
        private readonly IDictionary<EnumEntityValue, object> _map;

        public EnumValueMap(TProperty entityValue, EnumValuesMap<TEnum, TProperty> valuesMap, IDictionary<EnumEntityValue, object> map)
        {
            _entityValue = entityValue;
            _valuesMap = valuesMap;
            _map = map;
        }

        public virtual EnumValuesMap<TEnum, TProperty> Enum(TEnum enumValue)
        {
            return AddEnumValueMapToPropertyValue(_entityValue, enumValue);
        }

        protected EnumValuesMap<TEnum, TProperty> AddEnumValueMapToPropertyValue(object value, TEnum enumValue)
        {
            _map[new EnumEntityValue(value)] = enumValue;
            return _valuesMap;
        }
    }
}