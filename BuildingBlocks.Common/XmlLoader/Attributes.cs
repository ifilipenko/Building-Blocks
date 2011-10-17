using System;

namespace BuildingBlocks.Common.XmlLoader
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PreLoadUniqueCheckAttribute : Attribute
    {
        private string _errorText;
        private string _attributes;

        public PreLoadUniqueCheckAttribute () 
            : base()
        {
        }

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        public string Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ClassNodeAttribute : Attribute
    {
        public ClassNodeAttribute(string name)
            : base()
        {
            _nodeName = name;
        }

        readonly string _nodeName;

        public string NodeName
        {
            get { return _nodeName; }            
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ValueAttributeAttribute : Attribute
    {
        public ValueAttributeAttribute(string attributeName)        
            : base()
        {
            _attributeName = attributeName;
        }

        readonly string _attributeName;

        public string AttributeName
        {
            get { return _attributeName; }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RequaredAttribute : Attribute
    {
        public RequaredAttribute()
            : base()
        {            
        }       
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoRequaredAttribute : Attribute
    {
        public NoRequaredAttribute()
            : base()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ChildNodeAttribute : Attribute
    {
        public ChildNodeAttribute()
            : base()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ChildNodeCollectionAttribute : Attribute
    {
        public ChildNodeCollectionAttribute()
            : base()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Enum |  AttributeTargets.Field)]
    public class MapEnumAttribute : Attribute
    {
        readonly string _enumNode;

        public MapEnumAttribute(string enumNode)
            : base()
        {
            _enumNode = enumNode;
        }

        public string EnumNode
        {
            get { return _enumNode; }
        }
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class MappedEnumAttribute : Attribute
    {
        public MappedEnumAttribute()
            : base()
        {            
        }
    }
}