using System;
using AutoPoco.Configuration;
using AutoPoco.Engine;
using BuildingBlocks.TestHelpers.DataGenerator.DataSources;

namespace BuildingBlocks.TestHelpers.DataGenerator
{
    public static class EngineConfigurationTypeMemberBuilderHelpers
    {
        public static IEngineConfigurationTypeBuilder<TTarget> UseDependecySource<TTarget, TDependency>(
            this IEngineConfigurationTypeMemberBuilder<TTarget, TDependency> memberBuilder, 
            Func<IObjectGenerator<TDependency>, IObjectGenerator<TDependency>> dependencyGeneratorSetup = null)
        {
            return memberBuilder.Use<DependencySource<TDependency>>(typeof(TTarget), dependencyGeneratorSetup);
        }
    }
}