using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    public class XMLItemLoader
    {
        class XMLChildNodeLoader : XMLItemLoader
        {
            public XMLChildNodeLoader(Type itemType, XmlNode node) 
                : base(itemType, node)
            {
            }

            protected override void PerformLoad()
            {
                LoadClassInstanceFromSubNodes();
            }
        }

        readonly Type _itemType;
        readonly ConstructorInfo _defaultConstructor;
        readonly XmlNode _startLoadNode;
        readonly ArrayList _result;
        private FinderAttributes _itemAttrsFinder;
        private ClassNodeAttribute _classNodeAttr;

        protected XMLItemLoader(Type itemType, XmlNode node)
        {
            Debug.Assert(itemType != null, "ctor of XMLItemLoader: itemType parameter is null");
            Debug.Assert(node != null, "ctor of XMLItemLoader: node parameter is null");

            _defaultConstructor = GetDefaultConstructor(itemType);
            _itemType = itemType;
            _itemAttrsFinder = new FinderAttributes(_itemType);
            _startLoadNode = node;
            _result = new ArrayList();
            _classNodeAttr = _itemAttrsFinder.FindClassAttributeInstance<ClassNodeAttribute>();

            PerformLoad();
        }

        private ConstructorInfo GetDefaultConstructor(Type itemType)
        {
            ConstructorInfo defaultConstructor = itemType.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor == null)
            {
                throw new XMLLoaderException(string.Format("Item type [{0}] not have default constructor", itemType));
            }
            return defaultConstructor;
        }

        public Type ItemType
        {
            get { return _itemType; }
        }

        protected virtual void PerformLoad()
        {
            if (_classNodeAttr.NodeName == _startLoadNode.Name)
            {
                LoadClassInstanceFromCurrentNode();
            }
            else
            {
                LoadClassInstanceFromSubNodes();
            }
        }

        private void LoadClassInstanceFromCurrentNode()
        {
            object resultItem = CreateNewItem();
            LoadChilds(_startLoadNode, resultItem);
            AddToResult(resultItem);
        }

        protected void LoadClassInstanceFromSubNodes()
        {
            List<XmlNode> subNodes = SelectNodes(_startLoadNode, _classNodeAttr);
            foreach (XmlNode subNode in subNodes)
            {
                object resultItem = CreateNewItem();
                LoadChilds(subNode, resultItem);
                AddToResult(resultItem);
            }
        }

        private void LoadChilds(XmlNode nodeItem, object loadingInstance)
        {
            LoadAttrributes(nodeItem, loadingInstance);
            LoadCollectionsProperties(nodeItem, loadingInstance);
            LoadChildsProperties(nodeItem, loadingInstance);
        }

        #region [LoadAttrributes]

        private void LoadAttrributes(XmlNode nodeItem, object parentNodeEntity)
        {
            PropertyInfo[] attrsProperties = _itemAttrsFinder.FindAttributePropertys<ValueAttributeAttribute>();
            foreach (PropertyInfo property in attrsProperties)
            {
                bool isRequaredProperty = GetIsRequaredProperty(property);

                if (!property.CanWrite)
                {
                    throw new Exception(
                        string.Format("Can not write property \"{0}.{1}\"",
                                      parentNodeEntity.GetType(), property.Name));
                }

                XmlAttribute attribute = GetAttributeOfProperty(nodeItem, property, isRequaredProperty);
                if (attribute != null)
                {
                    object propertyValue = ConvertAttributeValue(attribute.Value, property.PropertyType);
                    property.SetValue(parentNodeEntity, propertyValue, new object[0]);
                }
            }
        }

        private static XmlAttribute GetAttributeOfProperty(XmlNode parentNodeItem, PropertyInfo property,
                                                           bool isRequaredProperty)
        {
            FinderAttributes finder = new FinderAttributes(property.DeclaringType);
            ValueAttributeAttribute valueAttribute =
                finder.FindAttributeInstance<ValueAttributeAttribute>(property);
            XmlAttribute attribute = parentNodeItem.Attributes[valueAttribute.AttributeName];

            if (attribute == null && isRequaredProperty)
            {
                throw new Exception(
                    string.Format("Expected attribute \"{0}\" for node \"{1}\" not found", valueAttribute.AttributeName,
                                  parentNodeItem.Name));
            }

            return attribute;
        }

        private static object ConvertAttributeValue(string attributeValue, Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                AttributeValue<MapEnumAttribute>[] mapEnumAttributes = GetEnumAttributes(propertyType);
                foreach (AttributeValue<MapEnumAttribute> attributeEnumValue in mapEnumAttributes)
                {
                    if (attributeEnumValue.Attribute.EnumNode == attributeValue)
                    {
                        return attributeEnumValue.Value;
                    }
                }
                throw new XMLLoaderException(
                    string.Format("String \"{0}\" not mapped to enum values \"{1}\"", attributeValue, propertyType));
            }
            return Convert.ChangeType(attributeValue, propertyType);
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

        #endregion

        #region [LoadChildsProperties]

        private void LoadChildsProperties(XmlNode parentNodeItem, object parentNodeEntity)
        {
            PropertyInfo[] childsProperties = _itemAttrsFinder.FindAttributePropertys<ChildNodeAttribute>();
            foreach (PropertyInfo property in childsProperties)
            {
                bool isRequaredProperty = GetIsRequaredProperty(property);

                if (!property.CanWrite)
                {
                    throw new Exception(string.Format("Can not write property \"{0}.{1}\"",
                                                      parentNodeEntity.GetType(), property.Name));
                }

                XmlNode childObjectNode = GetNodeOfProperty(parentNodeItem, property, isRequaredProperty);
                if (childObjectNode != null)
                {
                    XMLItemLoader childLoader = new XMLItemLoader(property.PropertyType, childObjectNode);
                    if (childLoader.LoadResult != null && childLoader.LoadResult.Count > 0)
                    {
                        property.SetValue(parentNodeEntity, childLoader.LoadResult[0], null);
                    }
                }
            }
        }

        private static XmlNode GetNodeOfProperty(XmlNode parentNodeItem, PropertyInfo property, bool isRequaredProperty)
        {
            FinderAttributes finder = new FinderAttributes(property.PropertyType);
            ClassNodeAttribute classNodeAttr = finder.FindClassAttributeInstance<ClassNodeAttribute>();
            XmlNode childObjectNode = parentNodeItem.SelectSingleNode(classNodeAttr.NodeName);

            if (childObjectNode == null)
            {
                if (isRequaredProperty)
                {
                    throw new Exception(
                        string.Format("Expected node \"{0}\" for node \"{1}\" not found", classNodeAttr.NodeName,
                                      parentNodeItem.Name));
                }
            }

            return childObjectNode;
        }

        #endregion

        #region [LoadCollectionsProperties]

        private void LoadCollectionsProperties(XmlNode loadingNode, object loadingInstance)
        {
            PropertyInfo[] arrayProperties = _itemAttrsFinder.FindAttributePropertys<ChildNodeCollectionAttribute>();
            foreach (PropertyInfo property in arrayProperties)
            {
                if (!property.PropertyType.IsArray)
                {
                    throw new Exception(string.Format(@"Expected array but was ""{0}""", 
                                                      property.PropertyType));
                }
                Type childClass = property.PropertyType.GetElementType();

                if (ChildsNodesIsEmpty(loadingNode, childClass))
                {
                    continue;
                }

                PreLoadAttributesUniqueValuesCheck(loadingNode, childClass);

                XMLChildNodeLoader childLoader = new XMLChildNodeLoader(childClass, loadingNode);
                Array propertyArrayValue = childLoader.LoadResult.ToArray(childClass);
                property.SetValue(loadingInstance, propertyArrayValue, new object[0]);
            }
        }

        private static bool ChildsNodesIsEmpty(XmlNode parentNodeItem, Type childClass)
        {
            FinderAttributes finderAttributes = new FinderAttributes(childClass);
            ClassNodeAttribute classNodeAttribute = finderAttributes.FindClassAttributeInstance<ClassNodeAttribute>();
            List<XmlNode> subNodes = SelectNodes(parentNodeItem, classNodeAttribute);
            return subNodes.Count == 0;
        }

        private static void PreLoadAttributesUniqueValuesCheck(XmlNode parentNodeItem, Type nodesTypeForCheck)
        {
            FinderAttributes finderAttributes = new FinderAttributes(nodesTypeForCheck);
            string checkingNodeName = finderAttributes.FindClassAttributeInstance<ClassNodeAttribute>().NodeName;
            PropertyInfo[] attributesForCheck =
                finderAttributes.FindAttributesPropertys<PreLoadUniqueCheckAttribute, ValueAttributeAttribute>();

            XmlNodeList nodesForCheck = parentNodeItem.SelectNodes(checkingNodeName);

            foreach (PropertyInfo propertyForCheck in attributesForCheck)
            {
                PreLoadUniqueCheckAttribute checkAttribute =
                    finderAttributes.FindAttributeInstance<PreLoadUniqueCheckAttribute>(propertyForCheck);
                ValueAttributeAttribute valueAttribute =
                    finderAttributes.FindAttributeInstance<ValueAttributeAttribute>(propertyForCheck);

                CheckNodeAttributeUnique(nodesForCheck, valueAttribute, checkAttribute.ErrorText);
            }
        }

        private static void CheckNodeAttributeUnique(XmlNodeList nodesForCheck, ValueAttributeAttribute valueAttribute, string errorMessage)
        {
            foreach (XmlNode childNode in nodesForCheck)
            {
                XmlAttribute attribute = childNode.Attributes[valueAttribute.AttributeName];
                if (attribute == null)
                    continue;
                foreach (XmlNode checkingNode in nodesForCheck)
                {
                    if (checkingNode != childNode)
                    {
                        XmlAttribute checkingAttribute = checkingNode.Attributes[valueAttribute.AttributeName];
                        if (checkingAttribute == null)
                            continue;
                        if (checkingAttribute.Value == attribute.Value)
                        {
                            throw new XMLLoaderUnqueCheckException(errorMessage);
                        }
                    }
                }
            }
        }

        #endregion

        private static List<XmlNode> SelectNodes(XmlNode parentNodeItem, ClassNodeAttribute classNodeAttribute)
        {
            List<XmlNode> subNodes = new List<XmlNode>(parentNodeItem.ChildNodes.Count);
            foreach (XmlNode childNode in parentNodeItem.ChildNodes)
            {
                if (childNode.Name == classNodeAttribute.NodeName)
                {
                    subNodes.Add(childNode);
                }
            }
            return subNodes;
        }
        
        private static bool GetIsRequaredProperty(PropertyInfo property)
        {
            FinderAttributes finder = new FinderAttributes(property.PropertyType);
            RequaredAttribute requaredAttr = finder.FindAttributeInstance<RequaredAttribute>(property);
            return requaredAttr != null;
        }

        protected virtual void AddToResult(object value)
        {
            _result.Add(value);
        }

        ArrayList LoadResult
        {
            get { return _result; }
        }

        object CreateNewItem()
        {
            return _defaultConstructor.Invoke(new object[0]);
        }
    }
}