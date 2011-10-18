using System;
using BuildingBlocks.Persistence.Exceptions;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal class EnumMapMetadata
    {
        private readonly Type _enumType;
        private readonly Type _enumEntity;
        private readonly Type _enumMapType;
        private readonly IEnumMap _enumMap;

        public EnumMapMetadata(Type enumType, Type enumEntity, Type enumMapType)
        {
            _enumType = enumType;
            _enumEntity = enumEntity;
            _enumMapType = enumMapType;

            _enumMap = (IEnumMap) Activator.CreateInstance(_enumMapType);
        }

        public Type EnumEntity
        {
            get { return _enumEntity; }
        }

        public Type EnumType
        {
            get { return _enumType; }
        }

        public string EnumEntityValueProperty
        {
            get { return _enumMap.EntityValueProperty.Name; }
        }

        public string EnumEntityIdProperty
        {
            get { return _enumMap.EntityIdProperty.Name; }
        }

        public object GetEnumEntityValueForEnum(ValueType @enum)
        {
            var enumEntityValue = _enumMap.GetEnumEntityValueForEnum(@enum);
            if (enumEntityValue == null)
            {
                throw new EnumToEntityMapException("Enum member \"" + @enum + "\" is not mapped");
            }
            return enumEntityValue.Value;
        }

        public ValueType GetEnumByEnumEntityValue(object value)
        {
            var @enum = _enumMap.GetEnumByEnumEntityValue(value);
            return @enum;
        }
    }
}