using FluentAssertions;

namespace BuildingBlocks.Persistence.TestHelpers.FixtureHelpers
{
    public static class AssertionsHelpers
    {
        public static void should_not_exists_entities_of<T>(this AssertionsBase assertions)
            where T : class
        {
            assertions.Db.GetCount<T>()
                .Should().Be(0, "entities of " + typeof(T) + " should not exists");
        }
    }
}