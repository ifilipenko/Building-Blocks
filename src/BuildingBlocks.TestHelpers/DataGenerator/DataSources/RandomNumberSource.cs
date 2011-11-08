using System;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.Exceptions;

namespace BuildingBlocks.TestHelpers.DataGenerator.DataSources
{
    public class RandomNumberSource<T> : IDatasource<T>
        where T : struct
    {
        private readonly int _from;
        private readonly int _to;
        private readonly Random _random;

        public RandomNumberSource(int from, int to)
        {
            _from = from;
            _to = to;
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public object Next(IGenerationContext context)
        {
            if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
            {
                var value = _random.Next();
                value = value * (_to - _from) + _from;
                return (T)(object)value;
            }
            if (typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(decimal))
            {
                var value = _random.Next(_from, _to + 1);
                return Convert.ChangeType(value, typeof(T));
            }

            throw new DataGeneratorException("Unexpected numeric type [" + typeof(T) + "]");
        }
    }
}