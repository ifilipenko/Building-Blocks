using BuildingBlocks.Common.Sugar;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public abstract class UnaryFilterNode : FilterNode
    {
        private FilterNode _innerNode;

        protected UnaryFilterNode(FilterNode node)
        {
            Condition.Requires(node, "node").IsNotNull();
            _innerNode = node;
            _innerNode.Parent = this;
        }

        public override bool IsInvalid
        {
            get { return InnerNode == null; }
        }

        public FilterNode InnerNode
        {
            get { return _innerNode; }
            set { _innerNode = value; }
        }

        public override System.Collections.Generic.IEnumerable<FilterNode> Children
        {
            get
            {
                if (InnerNode == null)
                {
                    return null;
                }
                return InnerNode.ToEnumerable();
            }
        }

        public override void MakeInvalid()
        {
            InnerNode = null;
        }
    }
}