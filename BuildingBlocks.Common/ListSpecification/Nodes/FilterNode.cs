using System.Collections.Generic;
using BuildingBlocks.Common.Utils.Hierarchy;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public abstract class FilterNode : IHierarchy<FilterNode>
    {
        public FilterNode Parent { get; set; }
        public abstract IEnumerable<FilterNode> Children { get; }
        public abstract TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor);
        public abstract FilterNode Clone();
        public abstract bool IsInvalid { get; }
        public abstract void MakeInvalid();
    }
}