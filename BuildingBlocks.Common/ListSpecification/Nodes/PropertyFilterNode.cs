using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public abstract class PropertyFilterNode : FilterNode
    {
        protected PropertyFilterNode(string propertyName)
        {
            Condition.Requires(propertyName, "propertyName").IsNotNull();
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }

        public override bool IsInvalid
        {
            get { return string.IsNullOrEmpty(PropertyName); }
        }

        public override void MakeInvalid()
        {
            PropertyName = null;
        }

        public override System.Collections.Generic.IEnumerable<FilterNode> Children
        {
            get { return null; }
        }

        public abstract void FillByFilterEntry(FilterEntry filterEntry);
        public abstract FilterEntry ToFilterEntry();
    }
}