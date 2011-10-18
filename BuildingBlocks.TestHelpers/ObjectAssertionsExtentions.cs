using System;
using FluentAssertions;
using FluentAssertions.Assertions;

namespace BuildingBlocks.TestHelpers
{
    public static class ObjectAssertionsExtentions
    {
        public static AndConstraint<ObjectAssertions> BeOfType(this ObjectAssertions objectAssertions, Type type)
        {
            return BeOfType(objectAssertions, type, string.Empty);
        }

        public static AndConstraint<ObjectAssertions> BeOfType(this ObjectAssertions objectAssertions,
            Type type,
            string reason,
            params object[] reasonParameters)
        {
            Execute.Verification.ForCondition(type == objectAssertions.GetType())
                .BecauseOf(reason, reasonParameters)
                .FailWith("Expected type {0}{reason}, but found {1}.",
                          type,
                          objectAssertions.Subject.GetType());
            return new AndConstraint<ObjectAssertions>(objectAssertions);
        }
    }
}