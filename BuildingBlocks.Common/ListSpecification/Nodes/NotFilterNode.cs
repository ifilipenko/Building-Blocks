namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class NotFilterNode : UnaryFilterNode
    {
        public NotFilterNode(FilterNode node) 
            : base(node)
        {
        }

        public override TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public override FilterNode Clone()
        {
            return new NotFilterNode(InnerNode.Clone());
        }
    }
}