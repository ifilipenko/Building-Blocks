using System;
using System.Reflection;
using FluentNHibernate.Mapping;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal interface IEnumMap
    {
        void CreateRulesForProperty<TEnumOwner>(PropertyPart propertyPart);
        PropertyInfo EntityValueProperty { get; }
        PropertyInfo EntityIdProperty { get; }
        EnumEntityValue GetEnumEntityValueForEnum(ValueType @enum);
        ValueType GetEnumByEnumEntityValue(object value);
    }
}