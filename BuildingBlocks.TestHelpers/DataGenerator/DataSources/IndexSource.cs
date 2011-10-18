using System;
using AutoPoco.Engine;
using BuildingBlocks.Common.Sugar;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class IndexSource<T> : IDatasource<T>
    {
        private readonly Func<decimal, T> _valueConverter;
        private decimal _index;
        private readonly decimal _increment;

        public IndexSource() 
            : this(null)
        {
        }

        public IndexSource(Func<decimal, T> indexValueConverter)
        {
            _valueConverter = indexValueConverter ?? DefaultIndexValueConverter;
            _index = 0;
            _increment = 1;
        }

        public IndexSource(decimal indexStartAt, decimal increment, Func<decimal, T> indexValueConverter)
            : this(indexValueConverter)
        {
            _index = indexStartAt;
            _increment = increment;
        }

        public object Next(IGenerationContext context)
        {
            var oldIndex = _index;
            _index += _increment;
            return _valueConverter(oldIndex);
        }

        private T DefaultIndexValueConverter(decimal index)
        {
            return index.ForcedCastTo<T>();
        }
    }
}