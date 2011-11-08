using System.Collections;
using System.Linq;
using FluentAssertions;

namespace BuildingBlocks.TestHelpers.Fixtures
{
    public class AssertionsFixturePart : FixturePart
    {
        public T InstanceAs<T>()
        {
            return (T) Instance;
        }

        internal object Instance { get; set; }
        public object Result { get; set; }

        public void should_return_null_result()
        {
            Result.Should().BeNull();
        }

        public void should_return_not_null_result()
        {
            Result.Should().NotBeNull();
        }

        public void should_return_collection_with_count(int count)
        {
            Result.Should().BeAssignableTo<IEnumerable>();
            Result.As<IEnumerable>().OfType<object>().Should().HaveCount(count);
        }

        public void should_return_empty_collection()
        {
            should_return_collection_with_count(0);
        }

        public void should_return_not_empty_collection()
        {
            Result.Should().BeAssignableTo<IEnumerable>();
            Result.As<IEnumerable>().Should().NotBeEmpty();
        }
    }
}