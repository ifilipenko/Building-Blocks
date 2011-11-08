using System;
using System.Collections;
using BuildingBlocks.Common.ListSpecification.Nodes;

namespace BuildingBlocks.Common.ListSpecification
{
    public class FilterEntry : ICloneable
    {
        private object _value;

        public FilterEntry()
        {
        }

        public FilterEntry(string property, CompareOperator compareOperator, object value)
        {
            Property = property;
            CompareOperator = compareOperator;
            _value = value;
        }
        
        public FilterEntry(PropertyValueFilterNode node)
            : this(node.PropertyName, node.CompareOperator, node.Value)
        {
        }

        public string Property { get; set; }
        public CompareOperator CompareOperator { get; set; }
        
        public object Value
        {
            get { return _value; }
            set
            {
                if (value != null && value is IEnumerable && !(value is string) && !(value is IList))
                {
                    throw new ArgumentException("For values list expected IList implementation, but geven " + value, "value");
                }
                _value = value;
            }
        }

        public override string ToString()
        {
            return string.Format("Property: {0}, CompareOperator: {1}, Value: {2}", Property, CompareOperator, Value);
        }

        public bool Equals(FilterEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Property, Property) &&
                   Equals(other.CompareOperator, CompareOperator) &&
                   Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) || obj.GetType() == typeof (FilterEntry) && Equals((FilterEntry) obj));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Property != null ? Property.GetHashCode() : 0);
                result = (result*397) ^ CompareOperator.GetHashCode();
                result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                return result;
            }
        }

        public virtual FilterEntry Clone()
        {
            return new FilterEntry
                       {
                           Property = Property,
                           CompareOperator = CompareOperator,
                           _value = Value
                       };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}