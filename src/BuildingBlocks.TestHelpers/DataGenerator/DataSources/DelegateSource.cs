using System;
using AutoPoco.Engine;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class DelegateSource<T> : IDatasource<T>
    {
        private readonly Func<IGenerationContext, T> _valueGetter;

        public DelegateSource(Func<IGenerationContext, T> valueGetter)
        {
            _valueGetter = valueGetter;
        }

        public object Next(IGenerationContext context)
        {
            return _valueGetter(context);
        }
    }
}