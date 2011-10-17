using System;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    class DataPropertyMember : NodeToChildMapper
    {
        private readonly string _attributeName;
        private readonly bool _isRequared;

        public DataPropertyMember(Type ownerClassToLoad, PropertyInfo propertyInfo) 
            : base(ownerClassToLoad, propertyInfo)
        {
            FinderAttributes finderAttributes = new FinderAttributes(ownerClassToLoad);
            ValueAttributeAttribute propertyAttribute = 
                finderAttributes.FindAttributeInstance<ValueAttributeAttribute>(propertyInfo);
            if (propertyAttribute == null)
            {
                throw new InvalidOperationException(string.Format("Для свойства [{0}] не указанн аттрибут [{1}]", propertyInfo.Name, typeof(ValueAttributeAttribute).Name));
            }
            _attributeName = propertyAttribute.AttributeName;
            _isRequared = finderAttributes.FindAttributeInstance<RequaredAttribute>(propertyInfo) != null;
        }

        public bool IsRequared
        {
            get { return _isRequared; }
        }

        public string AttributeName
        {
            get { return _attributeName; }
        }

        public void LoadAttribute(object ownerValue, XmlNode node)
        {
            XmlAttribute xmlAttribute = node.Attributes[AttributeName];
            if (xmlAttribute == null)
            {
                if (IsRequared)
                {
                    throw new InvalidOperationException(
                        string.Format("Аттрибут [{0}] не задан в узле [{1}]", AttributeName, node.Name));
                }
            }
            else
            {
                object attributeValue = ConvertToPropertyValue(xmlAttribute.Value);
                PropertyInfo.SetValue(ownerValue, attributeValue, null);
            }
        }

        private object ConvertToPropertyValue(string value)
        {
            if (PropertyInfo.PropertyType.IsEnum)
            {
                return MapEnumValue(value);
            }
            return Convert.ChangeType(value, PropertyInfo.PropertyType);
        }

        private object MapEnumValue(string value)
        {
            AttributeValue<MapEnumAttribute>[] mapEnumAttributes = 
                GetEnumAttributes(PropertyInfo.PropertyType);
            foreach (AttributeValue<MapEnumAttribute> attributeEnumValue in mapEnumAttributes)
            {
                if (attributeEnumValue.Attribute.EnumNode == value)
                {
                    return attributeEnumValue.Value;
                }
            }
            throw new XMLLoaderException(
                string.Format("String \"{0}\" not mapped to enum values \"{1}\"", value, 
                              PropertyInfo.PropertyType));
        }

        private static AttributeValue<MapEnumAttribute>[] GetEnumAttributes(Type propertyType)
        {
            FinderAttributes finder = new FinderAttributes(propertyType);
            MappedEnumAttribute mappedEnumAttribute = finder.FindEnumAttributeInstance<MappedEnumAttribute>();
            if (mappedEnumAttribute == null)
            {
                throw new XMLLoaderException(string.Format("Enum \"{0}\" not marked by attribute \"{1}\"", propertyType.Name, typeof(MappedEnumAttribute).Name));
            }
            return finder.FindEnumValueAttributesInstance<MapEnumAttribute>();
        }
    }
}