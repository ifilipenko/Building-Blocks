using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingBlocks.Common.XmlLoader
{
    public class AttributeValue<AttributeType>
    {
        private readonly AttributeType _attribute;
        private readonly object _value;

        public AttributeValue(AttributeType attribute, object value)
        {
            _attribute = attribute;
            _value = value;
        }

        public AttributeType Attribute
        {
            get { return _attribute; }
        }

        public object Value
        {
            get { return _value; }
        }
    }

    public class FinderAttributes
    {
        private const BindingFlags _propertysBindingFlags = 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        readonly Type _classType;
        private const bool _inherAttributes = false;

        public FinderAttributes(Type classType)
        {
            _classType = classType;
        }

        public PropertyInfo[] FindAttributePropertys<PropertyAttribute>()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            foreach (PropertyInfo property in _classType.GetProperties(_propertysBindingFlags))
            {
                if (FindAttributeInstance<PropertyAttribute>(property) != null)
                    result.Add(property);
            }
            return result.ToArray();
        }

        public PropertyInfo[] FindAttributesPropertys<PropertyAttribute1, PropertyAttribute2>()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            foreach (PropertyInfo property in _classType.GetProperties(_propertysBindingFlags))
            {
                object[] attributes = property.GetCustomAttributes(_inherAttributes);
                bool attr1Found = false;
                bool attr2Found = false;
                foreach (object attribute in attributes)
                {
                    if (!attr1Found)
                        attr1Found = attribute is PropertyAttribute1;
                    if (!attr2Found)
                        attr2Found = attribute is PropertyAttribute2;
                }
                if (attr1Found && attr2Found)
                    result.Add(property);
            }
            return result.ToArray();
        }

        public ClassAttribute FindClassAttributeInstance<ClassAttribute>()
        {
            object[] attributes = _classType.GetCustomAttributes(typeof(ClassAttribute), _inherAttributes);
            return (ClassAttribute)(attributes.Length > 0 ? attributes[0] : default(ClassAttribute));
        }

        public ClassAttribute[] FindClassAttributes<ClassAttribute>()
        {
            object[] attributes = _classType.GetCustomAttributes(typeof(ClassAttribute), _inherAttributes);
            ClassAttribute[] result = new ClassAttribute[attributes.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (ClassAttribute)attributes[i];
            }
            return result;
        }

        public PropertyAttribute FindAttributeInstance<PropertyAttribute>(PropertyInfo property)            
        {
            object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), _inherAttributes);
            return (PropertyAttribute)(attributes.Length > 0 ? attributes[0] : default(PropertyAttribute));
        }

        public EnumAttribute FindEnumAttributeInstance<EnumAttribute>()
        {
            object[] attributes = _classType.GetCustomAttributes(typeof(EnumAttribute), _inherAttributes);
            return (EnumAttribute)(attributes.Length > 0 ? attributes[0] : default(EnumAttribute));
        }

        public AttributeValue<MapEnumAttribute>[] FindEnumValueAttributesInstance<MapEnumAttribute>()
        {
            List<AttributeValue<MapEnumAttribute>> enumAttributeByValues = new List<AttributeValue<MapEnumAttribute>>();
            foreach (FieldInfo fieldInfo in _classType.GetFields())
            {
                if (fieldInfo.IsLiteral && fieldInfo.IsStatic)
                {
                    object[] attributes = fieldInfo.GetCustomAttributes(typeof(MapEnumAttribute), false);
                    if (attributes.Length == 0)
                    {
                        continue;
                    }
                    MapEnumAttribute attributeInstance = (MapEnumAttribute) attributes[0];
                    object enumValue = fieldInfo.GetValue(null);
                    enumAttributeByValues.Add(new AttributeValue<MapEnumAttribute>(attributeInstance, enumValue));
                }
            }
            return enumAttributeByValues.ToArray();
        }
    }
}