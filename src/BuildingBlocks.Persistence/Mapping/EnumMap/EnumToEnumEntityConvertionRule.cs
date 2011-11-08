using System;
using System.Collections.Generic;
using System.Reflection;
using BuildingBlocks.Persistence.Exceptions;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumToEnumEntityConvertionRule
    {
        private readonly Dictionary<EnumEntityValue, object> _valuesToEnumMap;
        private readonly PropertyInfo _resultProperty;
        private readonly Type _enumEntity;
        private readonly Type _resultType;
        private readonly Type _enumType;
        private readonly Dictionary<object, EnumEntityValue> _enumToValuesMap;
        private readonly Type _enumEntityIdentifierType;

        public EnumToEnumEntityConvertionRule(Dictionary<EnumEntityValue, object> valuesToEnumMap, PropertyInfo resultProperty, Type enumEntity, Type enumType, Type enumEntityIdentifierType)
        {
            Condition.Requires(valuesToEnumMap, "valuesToEnumMap").IsNotNull();
            Condition.Requires(resultProperty, "resultProperty").IsNotNull();
            Condition.Requires(enumType, "enumType").IsNotNull();
            Condition.Requires(enumEntityIdentifierType, "enumEntityIdentifierType").IsNotNull();

            if (!enumEntityIdentifierType.IsValueType)
            {
                throw new ArgumentException("Allowed enum entity with only value type indentifiers, but was " + enumEntityIdentifierType, "enumEntityIdentifierType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Expected enum type, but was " + enumType, "enumType");
            }

            _valuesToEnumMap = valuesToEnumMap;
            _enumEntityIdentifierType = enumEntityIdentifierType;
            _resultProperty = resultProperty;
            _enumEntity = enumEntity;
            _enumToValuesMap = new Dictionary<object, EnumEntityValue>(_valuesToEnumMap.Count);

            foreach (var key in _valuesToEnumMap.Keys)
            {
                var value = _valuesToEnumMap[key];
                _enumToValuesMap.Add(value, key);
            }
            _resultType = resultProperty.PropertyType;
            _enumType = enumType;
        }

        public Type EnumEntity
        {
            get { return _enumEntity; }
        }

        public PropertyInfo ResultProperty
        {
            get { return _resultProperty; }
        }

        public Type EnumType
        {
            get { return _enumType; }
        }

        public Type EnumValueType
        {
            get { return _resultType; }
        }

        public int StringLength { get; set; }

        public Type EnumEntityIdentifierType
        {
            get 
            {
                return _enumEntityIdentifierType;
            }
        }

        public bool HasEnumForNull
        {
            get
            {
                return _valuesToEnumMap.ContainsKey(new EnumEntityValue(null));
            }
        }

        public System.Enum ToEnum(object enumEntity)
        {
            return (System.Enum)_valuesToEnumMap[new EnumEntityValue(enumEntity)];
        }

        public System.Enum GetEnumForNull()
        {
            object enumValue;
            if (_valuesToEnumMap.TryGetValue(new EnumEntityValue(null), out enumValue))
            {
                return (System.Enum)enumValue;
            }
            throw new EnumToEntityMapException("Rule for null enum entity value not exists");
        }

        public object ToEntityValue(System.Enum enumValue)
        {
            Condition.Requires(enumValue, "enumValue")
                .IsOfType(_enumType, "Expected value of type " + _enumType);

            return _enumToValuesMap[enumValue].Value;
        }
    }
}