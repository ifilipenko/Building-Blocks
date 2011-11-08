using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public abstract class CompositeFilterNode : FilterNode
    {
        private readonly List<FilterNode> _childrenNodes;

        protected CompositeFilterNode(IEnumerable<FilterNode> childrenNodes)
        {
            _childrenNodes = childrenNodes.ToList();
            foreach (var childrenNode in _childrenNodes)
            {
                childrenNode.Parent = this;
            }
        }

        public override IEnumerable<FilterNode> Children
        {
            get { return _childrenNodes; }
        }

        public void AddNode(FilterNode node)
        {
            node.Parent = this;
            _childrenNodes.Add(node);
        }

        public void RemoveNode(FilterNode node)
        {
            if (_childrenNodes.Remove(node))
            {
                node.Parent = null;
            }
        }

        public override bool IsInvalid
        {
            get { return Children.Count() == 0; }
        }

        public override void MakeInvalid()
        {
            _childrenNodes.Clear();
        }
    }
}