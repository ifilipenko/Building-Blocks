namespace BuildingBlocks.Persistence.Mapping.EnumMap
{
    public class EnumEntityValue
    {
        public EnumEntityValue(object value)
        {
            Value = value;
            IsNull = ReferenceEquals(value, null);
        }

        public bool IsNull { get; private set; }
        public object Value { get; private set; }

        public bool Equals(EnumEntityValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EnumEntityValue)) return false;
            return Equals((EnumEntityValue) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}