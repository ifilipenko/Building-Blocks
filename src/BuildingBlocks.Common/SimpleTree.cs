using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Common
{
    public class SimpleTree<T>
    {
        public class SimpleTreeBuilder
        {
            private IEnumerable<T> _items;
            private System.Func<T, T> _parentSelector;
            private Predicate<T> _stopParentSelectionPredicate;

            internal SimpleTreeBuilder()
            {
            }

            internal SimpleTreeBuilder From(IEnumerable<T> items)
            {
                if (items == null)
                    throw new ArgumentNullException("items");

                _items = items;

                return this;
            }

            public SimpleTreeBuilder StopParentSelectionPredicate(Predicate<T> predicate)
            {
                if (predicate == null)
                    throw new ArgumentNullException("predicate");

                _stopParentSelectionPredicate = predicate;

                return this;
            }

            public SimpleTreeBuilder GroupBy(System.Func<T, T> parentSelector)
            {
                if (parentSelector == null)
                    throw new ArgumentNullException("parentSelector");

                _parentSelector = parentSelector;
                return this;
            }

            public IList<SimpleTree<T>> CreateTreeFrom()
            {
                if (_items == null)
                    throw new InvalidOperationException("Items not specified");
                if (_parentSelector == null)
                    throw new InvalidOperationException("Parent selector not specified");

                if (_stopParentSelectionPredicate == null)
                    _stopParentSelectionPredicate = i => Equals(_parentSelector(i), default(T));

                var grouping = _items
                    .GroupBy(_parentSelector)
                    .ToDictionary(g => g.Key, g => g.ToArray());

                IEnumerable<T> topParents = grouping
                    .Where(g => _stopParentSelectionPredicate(g.Key)).Select(g => g.Key);

                return ToTree(topParents, null, grouping).ToList();
            }

            private static IEnumerable<SimpleTree<T>> ToTree(IEnumerable<T> items, SimpleTree<T> parent, Dictionary<T, T[]> grouping)
            {
                foreach (var item in items)
                {
                    var node = new SimpleTree<T>
                               {
                                   Parent = parent,
                                   NodeItem = item,
                               };
                    node.Children = ToTree(grouping[item], node, grouping).ToArray();
                    yield return node;
                }
            }

            private SimpleTree<T> CreateNode(IGrouping<T, T> grouping)
            {
                var node = new SimpleTree<T>();
                node.NodeItem = grouping.Key;
                return node;
            }
        }

        public static SimpleTreeBuilder BuildTreeFrom(IEnumerable<T> items)
        {
            return new SimpleTreeBuilder().From(items);
        }

        public SimpleTree()
        {
        }

        public T NodeItem { get; set; }
        public SimpleTree<T> Parent;
        public SimpleTree<T>[] Children { get; set; }
    }
}