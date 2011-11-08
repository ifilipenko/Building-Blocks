using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common.Utils;
using BuildingBlocks.Common.VisitorInterfaces;

namespace BuildingBlocks.Common
{
    public abstract class Tree<TTree, TNode, TNodeItem> : IVisitorElement
        where TTree : Tree<TTree, TNode, TNodeItem>
        where TNode : Tree<TTree, TNode, TNodeItem>.Node
    {
        public class Node : IVisitorElement
        {
            private readonly List<TNode> _children = new List<TNode>();

            protected internal TNodeItem Item { get; set; }
            protected internal TTree Tree { get; set; }
            public TNode Parent { get; internal set; }

            public TNode Root
            {
                get
                {
                    if (IsRoot)
                        return (TNode) this;
                    return Parent.Root;
                }
            }

            public virtual IEnumerable<TNode> Children
            {
                get { return _children; }
            }

            public int Level
            {
                get
                {
                    if (IsRoot)
                        return 1;
                    return Parent.Level + 1;
                }
            }

            public bool IsRoot
            {
                get { return Parent == null; }
            }

            internal bool RemoveChild(TNode node)
            {
                return _children.Remove(node);
            }

            protected internal void AddChild(TNode node)
            {
                _children.Add(node);
                node.Tree = Tree;
                node.Parent = (TNode) this;
            }

            public void Accept(IVisitor visitor)
            {
                visitor.Visit(this);
            }

            internal bool MoveChild(TNode child, int offset)
            {
                if (child == null || child.Parent != this)
                    return false;
                return _children.MoveItem(child, offset);
            }
        }

        private readonly List<TNode> _rootNodes;
        private List<TNode> _flattenNodes;

        protected Tree()
        {
            _rootNodes = new List<TNode>(0);
        }

        public IEnumerable<TNode> RootNodes
        {
            get { return _rootNodes; }
        }

        public bool IsEmpty
        {
            get { return _rootNodes.Count() == 0; }
        }

        public IEnumerable<TNode> FlatNodesHierarhy
        {
            get { return EnsureFlatNodesHierarhyCache(); }
        }

        protected void AddNode(TNode parent, TNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (parent == null)
            {
                if (_rootNodes.Contains(node))
                {
                    throw new ArgumentException("Root node already contains in tree", "node");
                }
                _rootNodes.Add(node);
            }
            else
            {
                if (parent.Tree != this)
                {
                    throw new InvalidOperationException("Parent no contain in tree");
                }
                if (parent.Children.Contains(node))
                {
                    throw new ArgumentException("Node already contains in parent", "node");
                }

                parent.AddChild(node);
            }
            node.Tree = (TTree) this;
            node.Parent = parent;

            ResetFlatNodesHierarhyCache();
        }

        protected void RemoveNode(TNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (node.Tree != this)
                throw new ArgumentException("Node not contains in this tree", "node");

            if (node.IsRoot)
            {
                if (!_rootNodes.Remove(node))
                    return;
            }
            else if (!node.Parent.RemoveChild(node))
            {
                return;
            }

            node.Tree = null;
            ResetFlatNodesHierarhyCache();
        }

        protected bool MoveNode(TNode node, int direction)
        {
            return node.IsRoot
                       ? _rootNodes.MoveItem(node, direction)
                       : node.Parent.MoveChild(node, direction);
        }

        private void ResetFlatNodesHierarhyCache()
        {
            _flattenNodes = null;
        }

        private IEnumerable<TNode> EnsureFlatNodesHierarhyCache()
        {
            if (_flattenNodes == null)
            {
                _flattenNodes = new List<TNode>();
                AddToFlattenNode(RootNodes);
            }
            return _flattenNodes;
        }

        private void AddToFlattenNode(IEnumerable<TNode> nodes)
        {
            foreach (TNode node in nodes)
            {
                _flattenNodes.Add(node);
            }

            foreach (TNode node in nodes)
            {
                AddToFlattenNode(node.Children);
            }
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var node in RootNodes)
            {
                visitor.Visit(node);
            }
        }
    }
}