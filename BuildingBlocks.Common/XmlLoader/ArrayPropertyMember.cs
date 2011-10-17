using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    class ArrayPropertyMember : NodeToChildMapper
    {
        private readonly ClassMember _arrayItemMember;

        public ArrayPropertyMember(ClassMember ownerClassMember, PropertyInfo propertyInfo) 
            : base(ownerClassMember.ClassToLoad, propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsArray)
            {
                throw new ArgumentException("Expected array property type", "propertyInfo");
            }
            Type arrayItemType = propertyInfo.PropertyType.GetElementType();
            _arrayItemMember = arrayItemType == ownerClassMember.ClassToLoad
                                   ? ownerClassMember
                                   : new ClassMember(arrayItemType);
        }

        public void LoadArray(object ownerValue, XmlNode ownerNode)
        {
            ArrayList items = new ArrayList();
            foreach (XmlNode childNode in ownerNode)
            {
                if (childNode.Name == _arrayItemMember.ClassNodeName)
                {
                    object itemValue = _arrayItemMember.LoadValueFromNode(childNode);
                    items.Add(itemValue);
                }
            }
            Array propertyValue = Array.CreateInstance(_arrayItemMember.ClassToLoad, items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                propertyValue.SetValue(items[i], i);
            }
            PropertyInfo.SetValue(ownerValue, propertyValue, null);
        }
    }
}