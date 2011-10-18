using System.Collections.Generic;
using BuildingBlocks.Configuration;
using BuildingBlocks.TestHelpers.Dependencies;

namespace BuildingBlocks.TestHelpers
{
    public interface ITestConfiguration
    {
        IEnumerable<IConfigurationItem> GetItems(IDependencyAssembliesRegistry dependencyAssembliesRegistry);
        void AfterItemsApplying(IDependencyAssembliesRegistry dependencyAssembliesRegistry);
    }
}