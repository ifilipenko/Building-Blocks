using System;

namespace BuildingBlocks.TestHelpers.DataGenerator.InstancesModifier
{
    public interface IInstancesModifier<T>
    {
        IInstancesModifier<T> SetNextItems(int count, Action<T> itemModifier);
        IInstancesModifier<T> CyclicallyModifyItemsByRulesFromSet(params Action<T>[] rulesSet);
        IInstancesModifier<T> CyclicallyModifyItemsByRulesFromSet(int firstItemsCount, params Action<T>[] rulesSet);
    }
}