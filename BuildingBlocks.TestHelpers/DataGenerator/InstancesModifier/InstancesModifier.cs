using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CuttingEdge.Conditions;

namespace BuildingBlocks.TestHelpers.DataGenerator.InstancesModifier
{
    public class InstancesModifier<T> : IInstancesModifier<T>
    {
        private readonly T[] _items;
        private int _skip;

        public InstancesModifier(IEnumerable<T> enumerable)
        {
            _items = enumerable.ToArray();
            _skip = 0;
        }

        public IInstancesModifier<T> SetNextItems(int count, Action<T> itemModifier)
        {
            Contract.Requires(count > 0);
            Contract.Requires(itemModifier != null);
            if (_skip + count > _items.Length)
            {
                var message = string.Format("Requested items count {0} is more then left in collection {1}", 
                    count, _items.Length - _skip);
                throw new InvalidOperationException(message);
            }

            foreach (var item in _items.Skip(_skip).Take(count))
            {
                itemModifier(item);
            }
            _skip += count;

            return this;
        }

        public IInstancesModifier<T> CyclicallyModifyItemsByRulesFromSet(params Action<T>[] rulesSet)
        {
            return CyclicallyModifyItemsByRulesFromSet(_items.Length, rulesSet);
        }

        public IInstancesModifier<T> CyclicallyModifyItemsByRulesFromSet(int firstItemsCount, params Action<T>[] rulesSet)
        {
            Condition.Requires(firstItemsCount, "firstItemsCount")
                .IsGreaterThan(0)
                .IsLessOrEqual(_items.Length, "Top elements count should be less or equal to total items count");

            Condition.Requires(rulesSet, "rulesSet")
                .IsNotNull()
                .IsNotEmpty();

            int iRule = 0;
            for (int i = 0; i < firstItemsCount; i++)
            {
                if (iRule > rulesSet.Length - 1)
                {
                    iRule = 0;
                }

                var item = _items[i];
                var rule = rulesSet[iRule];

                rule(item);
                iRule++;
            }

            return this;
        }
    }
}