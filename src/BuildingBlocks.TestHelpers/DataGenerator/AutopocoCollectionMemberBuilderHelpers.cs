using System;
using System.Collections.Generic;
using AutoPoco.Configuration;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class AutopocoCollectionMemberBuilderHelpers 
    {
        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, IList<TItem>> memberBuilder, 
            int count,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(count, count, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, List<TItem>> memberBuilder, 
            int count,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder.Use<DependencyCollectionSource<TItem>>(count, count, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TItem[]> memberBuilder, 
            int count,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(count, count, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, ICollection<TItem>> memberBuilder, 
            int count,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(count, count, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, IList<TItem>> memberBuilder,
            int min, int max,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(min, max, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, List<TItem>> memberBuilder,
            int min, int max,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder.Use<DependencyCollectionSource<TItem>>(min, max, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TItem[]> memberBuilder,
            int min, int max,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(min, max, setUpCollection);
        }

        public static IEngineConfigurationTypeBuilder<TTarget> UseCollectionSource<TTarget, TItem>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, ICollection<TItem>> memberBuilder,
            int min, int max,
            Func<ICollectionContext<TItem, IList<TItem>>, ICollectionContext<TItem, IList<TItem>>> setUpCollection = null)
        {
            return memberBuilder
                .Use<DependencyCollectionSource<TItem>>(min, max, setUpCollection);
        }
    }
}