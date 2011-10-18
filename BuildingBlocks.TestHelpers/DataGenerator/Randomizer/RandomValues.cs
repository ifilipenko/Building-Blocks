using System;
using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;

namespace BuildingBlocks.TestHelpers.DataGenerator.Randomizer
{
    public class RandomValues
    {
        const int DefaultStringSize = 50;
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);

        public object Random(Type valueType)
        {
            return new RandomValueGenerator().GenerateValue(valueType, DefaultStringSize);
        }

        public int RandomInt(int from, int before)
        {
            Condition.Requires(before, "before").IsGreaterThan(from, "\"Before\" value should be greated then \"from\" value");

            return _random.Next(from, before);
        }

        public string RandomString(int size)
        {
            Condition.Requires(size, "size").IsGreaterThan(0);

            return (string) new RandomValueGenerator().GenerateValue(typeof (string), size);
        }

        public T Random<T>()
        {
            return (T) new RandomValueGenerator().GenerateValue(typeof(T), DefaultStringSize);
        }

        public string RandomCasing(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var random = new Random(DateTime.Now.Millisecond);

            var chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                var isUpper = random.NextDouble() > 0.5;
                c = isUpper ? char.ToUpper(c) : char.ToLower(c);
                chars[i] = c;
            }

            return new string(chars);
        }

        public T RandomItemFrom<T>(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            int count = items.Count();
            if (count == 0)
                throw new ArgumentException("Can not get index for empty collection", "items");

            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, count);
            return items.ElementAt(index);
        }
    }
}