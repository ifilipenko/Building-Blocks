using System;

namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    internal struct EnumToEnumEntityRuleKey
    {
        public Type EnumEntity { get; set; }
        public Type Enum { get; set; }
        public Type SourceEntity { get; set; }

        public bool Equals(EnumToEnumEntityRuleKey other)
        {
            return Equals(other.EnumEntity, EnumEntity) && Equals(other.Enum, Enum) && Equals(other.SourceEntity, SourceEntity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (EnumToEnumEntityRuleKey)) return false;
            return Equals((EnumToEnumEntityRuleKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (EnumEntity != null ? EnumEntity.GetHashCode() : 0);
                result = (result*397) ^ (Enum != null ? Enum.GetHashCode() : 0);
                result = (result*397) ^ (SourceEntity != null ? SourceEntity.GetHashCode() : 0);
                return result;
            }
        }
    }
}