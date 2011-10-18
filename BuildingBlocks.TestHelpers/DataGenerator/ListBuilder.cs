using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public class ListBuilder<T> : IListBuilder<T>
    {
        private readonly List<T> _list;

        public ListBuilder()
        {
            _list = new List<T>();
        }

        public IListBuilder<T> Items(int count, Func<int, T> itemFactory)
        {
            for (int i = 0; i < count; i++)
            {
                AddItem(itemFactory);
            }
            return this;
        }

        public IListBuilder<T> Item(Func<int, T> itemFactory)
        {
            AddItem(itemFactory);
            return this;
        }

        public List<T> GetList()
        {
            return _list.ToList();
        }

        public T[] GetArray()
        {
            return _list.ToArray();
        }

        private void AddItem(Func<int, T> itemFactory)
        {
            var index = _list.Count - 1;
            if (index < 0)
            {
                index = 0;
            }
            var item = itemFactory(index);
            _list.Add(item);
        }
    }
}