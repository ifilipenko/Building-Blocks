using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace BuildingBlocks.TestHelpers.Mocks
{
    public class MocksMethods
    {
        protected TCollection IsEquals<TCollection>(TCollection enumerable)
            where TCollection : IEnumerable
        {
            return It.Is<TCollection>(param => param.OfType<object>().SequenceEqual(enumerable.OfType<object>()));
        }

        protected TCollection IsContainsAll<TCollection>(TCollection enumerable)
            where TCollection : IEnumerable
        {
            return It.Is<TCollection>(param => enumerable.OfType<object>().All(i => param.OfType<object>().Contains(i)));
        }

        protected TCollection IsContains<T, TCollection>(T value)
            where TCollection : IEnumerable<T>
        {
            return It.Is<TCollection>(param => param.Contains(value));
        }

        protected T IsContainsIn<T>(IEnumerable<T> enumerable)
        {
            return It.Is<T>(param => enumerable.Contains(param));
        }
    }
}