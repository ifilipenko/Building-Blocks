using System.Reflection;

namespace BuildingBlocks.TestHelpers.Dependencies
{
    public interface IDependencyAssembliesRegistry
    {
        Assembly[] MappingAssemblies { get; }
        Assembly[] GenerationRulesAssemblies { get; }
        Assembly[] AutomapperMapsAssemblies { get; }
    }
}