using System;
using BuildingBlocks.Common;

namespace BuildingBlocks.Configuration
{
    public interface IComplexConfigurationItemsList
    {
        void IncludeAllItemsFromAssemblies(Action<AssembliesListBuilder> assembliesSelector);
    }
}