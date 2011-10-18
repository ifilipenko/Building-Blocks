using System.Collections;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Assertions;

namespace BuildingBlocks.TestHelpers
{
    public static class CollectionAssertionsExtentions
    {
        public static AndConstraint<CollectionAssertions<TSubject, TAssertions>> HaveCountAtLeast<TSubject, TAssertions>(
            this CollectionAssertions<TSubject, TAssertions> collectionAssertions,
            int expected) 
            where TAssertions : CollectionAssertions<TSubject, TAssertions> 
            where TSubject : IEnumerable
        {
            return HaveCountAtLeast(collectionAssertions, expected, string.Empty);
        }

        public static AndConstraint<CollectionAssertions<TSubject, TAssertions>> HaveCountAtLeast<TSubject, TAssertions>(
            this CollectionAssertions<TSubject, TAssertions> collectionAssertions, 
            int expected,
            string reason, 
            params object[] reasonParameters)
            where TAssertions : CollectionAssertions<TSubject, TAssertions>
            where TSubject : IEnumerable
        {
            collectionAssertions.Subject.Should().NotBeNull();
            var enumerable = collectionAssertions.Subject.Cast<object>();
            Execute.Verification.ForCondition(() => enumerable.Count() >= expected)
                .BecauseOf("expected items at least {0}, but found {1}.",
                           expected,
                           enumerable.Count())
                .FailWith(reason, reasonParameters);
            return new AndConstraint<CollectionAssertions<TSubject, TAssertions>>(collectionAssertions);
        }

    }
}