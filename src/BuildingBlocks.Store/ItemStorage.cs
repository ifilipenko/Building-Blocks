using System;
using System.Collections;
using System.Diagnostics.Contracts;

namespace BuildingBlocks.Store
{
    public class ItemStorage<TItem>
        where TItem : IDisposable
    {
        private readonly IDictionary _items;

        public ItemStorage(IDictionary items)
        {
            Contract.Requires(items != null);
            _items = items;
        }

        public TItem Get(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));

            return (TItem) _items[key];
        }

        public bool Contains(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));

            return _items.Contains(key);
        }

        public void Set(TItem item, string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));

            _items[key] = item;
        }

        public void Remove(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));

            _items.Remove(key);
        }

        public TItem Obtain(Func<TItem> itemFactory, string key)
        {
            Contract.Requires(itemFactory != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(key));

            if (_items.Contains(key))
            {
                return Get(key);
            }

            var item = itemFactory();
            Set(item, key);
            return item;
        }
    }
}