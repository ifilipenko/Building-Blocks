using System;
using AutoPoco.Configuration;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;
using BuildingBlocks.TestHelpers.DataGenerator.Randomizer;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class RandomMemberBuilderHelpers
    {
        public static IEngineConfigurationTypeBuilder<TTarget> RandomString<TTarget>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, string> memberBuilder, int size)
        {
            Func<RandomValues, string> valueGenerator = r => r.RandomString(size);
            return memberBuilder.Use<RandomValueSource<string>>(valueGenerator);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> RandomInt<TTarget>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, int> memberBuilder, int from, int to)
        {
            return memberBuilder.Use<RandomNumberSource<int>>(from, to);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> RandomLong<TTarget>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, long> memberBuilder, int from, int to)
        {
            return memberBuilder.Use<RandomNumberSource<long>>(from, to);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> RandomValue<TTarget, TMember>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TMember> memberBuilder)
            where TMember : struct
        {
            return memberBuilder.Use<RandomValueSource<TMember>>();
        }

        public static IEngineConfigurationTypeBuilder<TTarget> RandomValue<TTarget, TMember>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TMember?> memberBuilder)
            where TMember : struct
        {
            return memberBuilder.Use<RandomNullableStructSource<TMember>>();
        }

        public static IEngineConfigurationTypeBuilder<TTarget> RandomValueByRule<TTarget, TMember>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TMember> memberBuilder,
            Func<RandomValues, TMember> randomValueRule)
        {
            return memberBuilder.Use<RandomValueSource<TMember>>(randomValueRule);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> IndexValue<TTarget, TMember>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TMember> memberBuilder,
            decimal indexStartAt = 0, decimal increment = 1, Func<decimal, TMember> indexToValue = null)
            where TMember : struct
        {
            return memberBuilder.Use<IndexSource<TMember>>(indexStartAt, increment, indexToValue);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> IndexValue<TTarget>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, string> memberBuilder,
            decimal indexStartAt = 0, decimal increment = 1, Func<decimal, string> indexToValue = null)
        {
            return memberBuilder.Use<IndexSource<string>>(indexStartAt, increment, indexToValue);
        }
    }
}