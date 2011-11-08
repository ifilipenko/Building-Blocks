namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class PropertyValueFilterNode : PropertyFilterNode
    {
        public PropertyValueFilterNode(string propertyName, CompareOperator compareOperator, object value)
            : base(propertyName)
        {
            CompareOperator = compareOperator;
            Value = value;
        }

        public CompareOperator CompareOperator { get; set; }
        public object Value { get; set; }

        public override TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public override FilterNode Clone()
        {
            return new PropertyValueFilterNode(PropertyName, CompareOperator, Value);
        }

        public override void FillByFilterEntry(FilterEntry filterEntry)
        {
            PropertyName = filterEntry.Property;
            CompareOperator = filterEntry.CompareOperator;
            Value = filterEntry.Value;
        }

        public override FilterEntry ToFilterEntry()
        {
            return new FilterEntry(this);
        }
    }
}