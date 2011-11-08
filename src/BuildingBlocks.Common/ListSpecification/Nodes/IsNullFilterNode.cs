namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class IsNullFilterNode : PropertyFilterNode
    {
        public IsNullFilterNode(string propertyName) 
            : base(propertyName)
        {
        }

        public override TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public override FilterNode Clone()
        {
            return new IsNullFilterNode(PropertyName);
        }

        public override void FillByFilterEntry(FilterEntry filterEntry)
        {
            PropertyName = filterEntry.Property;
        }

        public override FilterEntry ToFilterEntry()
        {
            return new FilterEntry
                       {
                           Property = PropertyName,
                           CompareOperator = CompareOperator.EqualTo,
                           Value = null
                       };
        }
    }
}