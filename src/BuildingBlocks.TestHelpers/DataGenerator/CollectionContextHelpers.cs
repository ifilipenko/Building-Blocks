using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using AutoPoco.Engine;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class CollectionContextHelpers
    {
        public static ICollectionContext<TPoco, TCollection> ImposeEach<TPoco, TCollection, TMember>(
            this ICollectionContext<TPoco, TCollection> collectionContext, 
            Expression<Func<TPoco, TMember>> propertyExpr,
            IEnumerable<TMember> items)
            where TCollection : ICollection<TPoco>
        {
            Contract.Requires(collectionContext != null);
            Contract.Requires(propertyExpr != null);
            Contract.Requires(items != null);

            var itemList = items.ToList();
            if (itemList.Count == 0)
                return collectionContext;

            var sequenceSelectionContext = collectionContext
                .First(1).Impose(propertyExpr, itemList.First());
            foreach (var item in itemList.Skip(1))
            {
                sequenceSelectionContext = sequenceSelectionContext
                    .Next(1).Impose(propertyExpr, item);
            }

            return sequenceSelectionContext.All();
        }
    }
}