using System;
using System.Collections.Generic;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public interface IListBuilder<T>
    {
        IListBuilder<T> Items(int itemsCount, Func<int, T> itemFactory);
        IListBuilder<T> Item(Func<int, T> itemFactory);
        List<T> GetList();
        T[] GetArray();
    }
}