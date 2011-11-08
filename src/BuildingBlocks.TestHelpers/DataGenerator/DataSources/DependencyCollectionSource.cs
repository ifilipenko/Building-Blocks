using System;
using System.Collections.Generic;
using System.Linq;
using AutoPoco.Engine;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class DependencyCollectionSource<TItem> : 
        IDatasource<IEnumerable<TItem>>,
        IDatasource<List<TItem>>, 
        IDatasource<IList<TItem>>, 
        IDatasource<TItem[]>, 
        IDatasource<ICollection<TItem>>
    {
        private readonly int _minCount;
        private readonly int _maxCount;
        private readonly Action<ICollectionContext<TItem, IList<TItem>>> _setUpCollection;

        public DependencyCollectionSource(int count)
            : this(count, count, null)
        {
        }

        public DependencyCollectionSource(int minCount, int maxCount, 
            Action<ICollectionContext<TItem, IList<TItem>>> setUpCollection)
        {
            _minCount = minCount;
            _maxCount = maxCount;
            _setUpCollection = setUpCollection ?? EmptySetUpCollection;
        }

        object IDatasource.Next(IGenerationContext context)
        {
            var random = new Random();
            var count = random.Next(_minCount, _maxCount + 1);
            var items = context.Collection(count, _setUpCollection);

            return ConvertResultToExpectedColletionType(GetCollectionType(context), items);
        }

        private static Type GetCollectionType(IGenerationContext context)
        {
            if (context.Node is TypeFieldGenerationContextNode)
            {
                return context.Node.CastTo<TypeFieldGenerationContextNode>().Field.FieldInfo.FieldType;
            }
            if (context.Node is TypePropertyGenerationContextNode)
            {
                return context.Node.CastTo<TypePropertyGenerationContextNode>().Property.PropertyInfo.PropertyType;
            }
            if (context.Node is TypeGenerationContextNode)
            {
                return context.Node.CastTo<TypeGenerationContextNode>().Target.GetType();
            }
            return typeof (IEnumerable<TItem>);
        }

        private static IEnumerable<TItem> ConvertResultToExpectedColletionType(Type collectionType, IEnumerable<TItem> items)
        {
            if (collectionType == typeof(IList<TItem>) || collectionType == typeof(ICollection<TItem>))
            {
                return items;
            }
            if (collectionType == typeof(List<TItem>))
            {
                return items.ToList();
            }
            if (collectionType == typeof(TItem[]))
            {
                return items.ToArray();
            }

            return items;
        }

        private void EmptySetUpCollection(ICollectionContext<TItem, IList<TItem>> collectionContext)
        {
        }
    }
}