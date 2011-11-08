using System.Collections.Generic;

namespace BuildingBlocks.Common.Utils.Hierarchy
{
    public interface IHierarchy<T>
        where T : class, IHierarchy<T>
    {
        T Parent { get; }
        IEnumerable<T> Children { get; }
    }
}