using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Common.Utils;
using BuildingBlocks.Persistence.Exceptions;
using CuttingEdge.Conditions;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumMap<TEnum, TEnumEntity> : IEnumMap
        where TEnum : struct
        where TEnumEntity : ICacheableEntity
    {
        private readonly Dictionary<EnumEntityValue, object> _valuesMap = new Dictionary<EnumEntityValue, object>();
        private PropertyInfo _enumEntityIdProperty;
        private PropertyInfo _enumEntityValueProperty;

        public EnumMap<TEnum, TEnumEntity> EntityId<TId>(Expression<Func<TEnumEntity, TId>> idSelector)
            where TId : struct
        {
            Condition.Requires(idSelector, "idSelector").IsNotNull();
            _enumEntityIdProperty = (PropertyInfo) idSelector.GetMemberInfo();
            return this;
        }

        public EnumValuesMap<TEnum, TProp> MapValuesBy<TProp>(Expression<Func<TEnumEntity, TProp>> propertySelector)
            where TProp : struct
        {
            Condition.Requires(propertySelector, "propertySelector").IsNotNull();
            _enumEntityValueProperty = (PropertyInfo) propertySelector.GetMemberInfo();
            return new EnumValuesMap<TEnum, TProp>(_valuesMap);
        }

        public EnumValuesMap<TEnum, string> MapValuesBy(Expression<Func<TEnumEntity, string>> propertySelector)
        {
            Condition.Requires(propertySelector, "propertySelector").IsNotNull();
            _enumEntityValueProperty = (PropertyInfo) propertySelector.GetMemberInfo();
            return new EnumValuesMap<TEnum, string>(_valuesMap);
        }

        void IEnumMap.CreateRulesForProperty<TEnumOwner>(PropertyPart propertyPart)
        {
            Condition.Requires(propertyPart, "propertyPart").IsNotNull();
            if (_enumEntityValueProperty == null)
                throw new InvalidOperationException("Before applying enum entity map value property should be initialized");
            if (_enumEntityIdProperty == null)
                throw new InvalidOperationException("Before applying enum entity map id property should be initialized");

            var rule = new EnumToEnumEntityConvertionRule(
                _valuesMap, 
                _enumEntityValueProperty,
                typeof (TEnumEntity),
                typeof (TEnum),
                _enumEntityIdProperty.PropertyType);
            var ruleKey = new EnumToEnumEntityRuleKey
                              {
                                  Enum = typeof(TEnum),
                                  EnumEntity = typeof(TEnumEntity),
                                  SourceEntity = typeof(TEnumOwner)
                              };
            EnumToEnumEntityRuleLocator.Get().AddRule(ruleKey, rule);

            if (rule.HasEnumForNull)
            {
                propertyPart.Nullable();
            }
            else
            {
                propertyPart.Not.Nullable();
            }
            propertyPart
                .CustomType(typeof (EnumToEnumEntityType<TEnum, TEnumOwner, TEnumEntity>));
        }

        PropertyInfo IEnumMap.EntityValueProperty
        {
            get { return _enumEntityValueProperty; }
        }

        PropertyInfo IEnumMap.EntityIdProperty
        {
            get { return _enumEntityIdProperty; }
        }

        EnumEntityValue IEnumMap.GetEnumEntityValueForEnum(ValueType @enum)
        {
            return _valuesMap.Where(p => Equals(p.Value, @enum)).Select(p => p.Key).FirstOrDefault();
        }

        ValueType IEnumMap.GetEnumByEnumEntityValue(object value)
        {
            object @enum;
            var enumEntityValue = new EnumEntityValue (value);
            if (_valuesMap.TryGetValue(enumEntityValue, out @enum))
            {
                return (ValueType) @enum;
            }
            throw new EnumToEntityMapException("Not found Enum for entity value " + value);
        }
    }
}