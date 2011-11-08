using System;
using System.Threading;

namespace BuildingBlocks.Persistence.Scope
{
    class ThreadLocatorContext : ISessionLocatorContext
    {
        private readonly string _key;
        private readonly LocalDataStoreSlot _slot;

        public ThreadLocatorContext(string key)
        {
            _key = key;
            _slot = Thread.GetNamedDataSlot(key);
        }

        public SessionLocatorItem Item
        {
            get { return Thread.GetData(_slot) as SessionLocatorItem; }
            set { Thread.SetData(_slot, value); }
        }

        public void RemoveItem()
        {
            Thread.SetData(_slot, null);
        }

    }
}