using System.Reflection;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    class DelegateProperyMember : NodeToChildMapper
    {
        private readonly ClassMember _innerToClassMember;
        private readonly bool _isRequared;

        public DelegateProperyMember(ClassMember ownerClassMember, PropertyInfo propertyInfo) 
            : base(ownerClassMember.ClassToLoad, propertyInfo)
        {
            FinderAttributes finderAttributes = new FinderAttributes(ownerClassMember.ClassToLoad);
            _isRequared = finderAttributes.FindAttributeInstance<RequaredAttribute>(propertyInfo) != null;

            if (ownerClassMember.ClassToLoad == propertyInfo.PropertyType)
            {
                _innerToClassMember = ownerClassMember;
            }
            else
            {
                _innerToClassMember = new ClassMember(propertyInfo.PropertyType);
            }
        }

        public bool IsRequared
        {
            get { return _isRequared; }
        }

        public ClassMember InnerToClassMember
        {
            get { return _innerToClassMember; }
        }

        public void LoadDelegate(object ownerValue, XmlNode ownerNode)
        {
            XmlNode delegateNode = ownerNode.SelectSingleNode(InnerToClassMember.ClassNodeName);
            if (delegateNode == null)
            {
                if (IsRequared)
                {
                    throw new NotFoundRequaredNodeException(
                        string.Format("Requared delegate class node with name \"{0}\"", InnerToClassMember.ClassNodeName));
                }
                return;
            }
            object propertyValue = InnerToClassMember.LoadValueFromNode(delegateNode);
            PropertyInfo.SetValue(ownerValue, propertyValue, null);
        }
    }
}