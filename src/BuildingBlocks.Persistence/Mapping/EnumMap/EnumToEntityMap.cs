using System;
using System.Collections.Generic;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumValuesMap<TEnum, TProperty>
        where TEnum : struct
    {
        private readonly IDictionary<EnumEntityValue, object> _map;

        public EnumValuesMap(IDictionary<EnumEntityValue, object> map)
        {
            _map = map;
        }

        public EnumValueMap<TEnum, TProperty> For(TProperty value)
        {
            return new EnumValueMap<TEnum, TProperty>(value, this, _map);
        }

        public EnumValueMap<TEnum, TProperty> ForNull
        {
            get { return new NullEntityValueEnumMap<TEnum, TProperty>(this, _map); }
        }

        public void MapEntityValueToEnumValuesByConvertEnumToInt()
        {
            throw new NotImplementedException();
        }
    }
}