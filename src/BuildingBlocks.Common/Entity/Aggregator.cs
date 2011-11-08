using System;
using System.Collections.Generic;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.Sugar;
using BuildingBlocks.Common.Utils;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.Entity
{
    public class Aggregator<TRoot>
        where TRoot : class
    {
        class ItemModel<TItem>
            where TItem : class
        {
            private readonly Func<TItem, TRoot> _itemRootGetter;
            private readonly Action<TItem, TRoot> _itemRootSetter;
            private readonly Func<TRoot, ICollection<TItem>> _itemsGetter;

            public ItemModel(Func<TRoot, ICollection<TItem>> itemsGetter, Func<TItem, TRoot> itemRootGetter, Action<TItem, TRoot> itemRootSetter)
            {
                _itemRootGetter = itemRootGetter;
                _itemsGetter = itemsGetter;
                _itemRootSetter = itemRootSetter;
            }

            public TRoot GetItemRoot(TItem item)
            {
                return _itemRootGetter(item);
            }

            public void SetItemRoot(TItem item, TRoot root)
            {
                _itemRootSetter(item, root);
            }

            public ICollection<TItem> GetItems(TRoot root)
            {
                return _itemsGetter(root);
            }
        }

        class ItemModelFactory
        {
            private readonly Dictionary<Type, object> _models = new Dictionary<Type, object>();
            
            public void AddItemModel<TItem>(
                Func<TRoot, ICollection<TItem>> itemsGetter, 
                Func<TItem, TRoot> itemRootGetter,
                Action<TItem, TRoot> itemRootSetter)
                where TItem : class
            {
                _models[typeof(TItem)] = new ItemModel<TItem>(itemsGetter, itemRootGetter, itemRootSetter);
            }

            public ItemModel<TItem> GetItemModel<TItem>()
                where TItem : class
            {
                object model;
                if (!_models.TryGetValue(typeof (TItem), out model))
                {
                    throw new ItemModelNotFoundException(typeof(TItem));
                }

                return model as ItemModel<TItem>;
            }
        }

        private readonly TRoot _root;
        private readonly ItemModelFactory _itemModelFactory;

        public Aggregator(TRoot root)
        {
            _root = root;
            _itemModelFactory = new ItemModelFactory();
        }

        public void AddItemAggregationRules<TItem>(Func<TRoot, ICollection<TItem>> itemsGetter, Func<TItem, TRoot> itemRootGetter, Action<TItem, TRoot> itemRootSetter)
            where TItem : class
        {
            _itemModelFactory.AddItemModel(itemsGetter, itemRootGetter, itemRootSetter);
        }

        public void AddItemToAggregate<TItem>(TItem item)
            where TItem : class
        {
            Condition.Requires(item, "item").IsNotNull();

            var itemModel = _itemModelFactory.GetItemModel<TItem>();
            var items = itemModel.GetItems(_root);

            if (items.FindIndex(x => ReferenceEquals(x, item)) > -1)
            {
                throw new ItemAlreadyExistsInRootException();
            }

            items.Add(item);
            itemModel.SetItemRoot(item, _root);
        }

        public void RemoveItemFromAggegate<TItem>(TItem item)
            where TItem : class
        {
            Condition.Requires(item, "item").IsNotNull();

            var itemModel = _itemModelFactory.GetItemModel<TItem>();
            var items = itemModel.GetItems(_root);
            var itemRoot = itemModel.GetItemRoot(item);

            Condition.Requires(itemRoot, "item").IsNotNull();

            if (!itemRoot.IsSame(_root))
            {
                throw new ItemIsNotContainedInRootException();
            }

            if (items is IList<TItem>)
            {
                var itemList = (IList<TItem>)items;
                int index = itemList.FindIndex(c => ReferenceEquals(c, item));
                if (index > -1)
                {
                    itemList.RemoveAt(index);
                    itemModel.SetItemRoot(item, null);
                }
            }
            else if (items.Remove(item))
            {
                itemModel.SetItemRoot(item, null);
            }
        }
    }
}