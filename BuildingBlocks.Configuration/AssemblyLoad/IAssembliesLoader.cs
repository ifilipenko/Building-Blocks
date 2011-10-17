using System.Reflection;

namespace BuildingBlocks.Configuration.AssemblyLoad
{
    public interface IAssembliesLoader
    {
        Assembly[] LoadAssemblies();
    }
}