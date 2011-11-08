using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.Common.ListSpecification.Nodes
{
    public class NodesTree
    {
        private FilterNode _root;

        public NodesTree(FilterNode root)
        {
            _root = root;
        }

        public FilterNode Root
        {
            get { return _root; }
        }

        public void RemoveNode(FilterNode filterNode)
        {
            if (filterNode.Parent == null)
            {
                if (filterNode.IsSame(_root))
                {
                    _root = null;
                }
            }

            if (filterNode.Parent is CompositeFilterNode)
            {
                filterNode.Parent.CastTo<CompositeFilterNode>().RemoveNode(filterNode);
            }
            else if (filterNode.Parent is UnaryFilterNode)
            {
                filterNode.Parent.CastTo<UnaryFilterNode>().InnerNode = null;
            }
            filterNode.Parent = null;
        }

        public void AddNodeToParent(FilterNode parent, FilterNode childNode)
        {
            if (parent == null)
            {
                _root = parent;
            }
            else if (parent is CompositeFilterNode)
            {
                parent.CastTo<CompositeFilterNode>().AddNode(childNode);
            }
            else if (parent is UnaryFilterNode)
            {
                parent.CastTo<UnaryFilterNode>().InnerNode = childNode;
            }

            childNode.Parent = parent;
        }

        public TResult Accept<TResult>(IFilterNodeVisitor<TResult> visitor)
        {
            return _root.Accept(visitor);
        }
    }
}