using System;

namespace BuildingBlocks.Common.ListSpecification
{
    public class InvalidPropertyFromListSpecificationMappingException : Exception
    {
        public InvalidPropertyFromListSpecificationMappingException(Type sourceType, Type targetType, string mappedProperty)
        {
            SourceType = sourceType;
            TargetType = targetType;
            MappedProperty = mappedProperty;
        }
        
        public InvalidPropertyFromListSpecificationMappingException(Type sourceType, Type targetType, string mappedProperty, Exception innerException)
            : base("Error", innerException)
        {
            SourceType = sourceType;
            TargetType = targetType;
            MappedProperty = mappedProperty;
        }

        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public string MappedProperty { get; set; }

        public override string Message
        {
            get
            {
                return string.Format(
                    "Invalid mapping from [{0}] to [{1}]. Property with name [{2}] not found in target type [{1}]",
                    SourceType.FullName, TargetType.FullName, MappedProperty);
            }
        }
    }
}