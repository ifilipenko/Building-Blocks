using System.Linq;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class InvalidNodesCleaner : IFilterNodeVisitor<object>
    {
        private NodesTree _tree;

        public FilterNode Process(FilterNode filterNode)
        {
            _tree = new NodesTree(filterNode);
            filterNode.Accept(this);
            return _tree.Root;
        }

        object IFilterNodeVisitor<object>.Visit(GroupFilterNode node)
        {
            foreach (var child in node.Children.ToList())
            {
                if (child.Parent.IsNotSame(node))
                {
                    child.Parent = node;
                }
                child.Accept(this);
            }

            RemoveNodeIfInvalid(node);
            return null;
        }

        object IFilterNodeVisitor<object>.Visit(IsNullFilterNode node)
        {
            RemoveNodeIfInvalid(node);
            return null;
        }

        object IFilterNodeVisitor<object>.Visit(NotFilterNode node)
        {
            RemoveNodeIfInvalid(node);
            return null;
        }

        object IFilterNodeVisitor<object>.Visit(PropertyValueFilterNode node)
        {
            RemoveNodeIfInvalid(node);
            return null;
        }

        private void RemoveNodeIfInvalid(FilterNode filterNode)
        {
            if (filterNode.IsInvalid)
            {
                _tree.RemoveNode(filterNode);
            }
        }
    }
}