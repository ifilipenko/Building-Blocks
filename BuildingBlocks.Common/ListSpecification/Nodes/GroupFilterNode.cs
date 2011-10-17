using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class GroupFilterNode : CompositeFilterNode
    {
        public GroupFilterNode(GroupOperation groupOperation, IEnumerable<FilterNode> childrenNodes)
            : base(childrenNodes)
        {
            GroupOperation = groupOperation;
        }

        public GroupOperation GroupOperation { get; set; }

        public override TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        public override FilterNode Clone()
        {
            return new GroupFilterNode(GroupOperation, Children.Select(c => c.Clone()));
        }
    }
}