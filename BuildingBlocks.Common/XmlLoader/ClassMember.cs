using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    class ClassMember
    {
        private readonly Type _classToLoad;
        private readonly string _classNodeName;
        private readonly List<DataPropertyMember> _dataPropertyMembers;
        private readonly List<DelegateProperyMember> _delegateProperyMembers;
        private readonly List<ArrayPropertyMember> _arrayPropertyMembers;

        public ClassMember(Type classToLoad)
        {
            _classToLoad = classToLoad;
            FinderAttributes finderAttributes = new FinderAttributes(classToLoad);
            _classNodeName = finderAttributes.FindClassAttributeInstance<ClassNodeAttribute>().NodeName;

            _dataPropertyMembers = GetDataPropertyMembers(finderAttributes);
            _delegateProperyMembers = GetDelegateProperyMembers(finderAttributes);
            _arrayPropertyMembers = GetArrayPropertyMembers(finderAttributes);
        }

        public List<ArrayPropertyMember> ArrayPropertyMembers
        {
            get { return _arrayPropertyMembers; }
        }

        public List<DelegateProperyMember> DelegateProperyMembers
        {
            get { return _delegateProperyMembers; }
        }

        public List<DataPropertyMember> DataPropertyMembers
        {
            get { return _dataPropertyMembers; }
        }

        public string ClassNodeName
        {
            get { return _classNodeName; }
        }

        public Type ClassToLoad
        {
            get { return _classToLoad; }
        }

        private List<ArrayPropertyMember> GetArrayPropertyMembers(FinderAttributes finderAttributes)
        {
            PropertyInfo[] childsProperties =
                finderAttributes.FindAttributePropertys<ChildNodeCollectionAttribute>();
            List<ArrayPropertyMember> classProperties = new List<ArrayPropertyMember>();
            foreach (PropertyInfo propertyInfo in childsProperties)
            {
                classProperties.Add(new ArrayPropertyMember(this, propertyInfo));
            }
            return classProperties;
        }

        private List<DelegateProperyMember> GetDelegateProperyMembers(FinderAttributes finderAttributes)
        {
            PropertyInfo[] childsProperties =
                finderAttributes.FindAttributePropertys<ChildNodeAttribute>();
            List<DelegateProperyMember> classProperties = new List<DelegateProperyMember>();
            foreach (PropertyInfo propertyInfo in childsProperties)
            {
                classProperties.Add(new DelegateProperyMember(this, propertyInfo));
            }
            return classProperties;
        }

        private List<DataPropertyMember> GetDataPropertyMembers(FinderAttributes finderAttributes)
        {
            PropertyInfo[] attributes = 
                finderAttributes.FindAttributePropertys<ValueAttributeAttribute>();

            List<DataPropertyMember> attributesProperties = new List<DataPropertyMember>();
            foreach (PropertyInfo propertyInfo in attributes)
            {
                attributesProperties.Add(new DataPropertyMember(_classToLoad, propertyInfo));
            }
            return attributesProperties;
        }

        public object LoadValueFromNode(XmlNode node)
        {
            Debug.Assert(node.Name == ClassNodeName);

            object value = ClassToLoad.GetConstructor(Type.EmptyTypes).Invoke(null);

            foreach (DataPropertyMember attributeToPropertyMapper in _dataPropertyMembers)
            {
                attributeToPropertyMapper.LoadAttribute(value, node);
            }
            foreach (DelegateProperyMember nodeToProperyClassMapper in _delegateProperyMembers)
            {
                nodeToProperyClassMapper.LoadDelegate(value, node);
            }
            foreach (ArrayPropertyMember nodeToArrayMapper in _arrayPropertyMembers)
            {
                nodeToArrayMapper.LoadArray(value, node);
            }

            return value;
        }
    }
}