using System;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Randomizer;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class RandomValueSource<T> : IDatasource<T>
    {
        private readonly Func<RandomValues, T> _valueGetter;
        private static readonly RandomValues _randomValues = new RandomValues();

        public RandomValueSource()
        {
            _valueGetter = GetRandomValue;
        }

        public RandomValueSource(Func<RandomValues, T> valueGetter)
        {
            _valueGetter = valueGetter;
        }

        public object Next(IGenerationContext context)
        {
            return _valueGetter(_randomValues);
        }

        private T GetRandomValue(RandomValues randomValues)
        {
            return randomValues.Random<T>();
        }
    }
}