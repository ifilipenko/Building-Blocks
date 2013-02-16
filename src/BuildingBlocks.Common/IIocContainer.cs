using System;

namespace BuildingBlocks.Common
{
    public interface IIocContainer
    {
        T Resolve<T>();
        object Resolve(Type type);
        T TryResolve<T>()
            where T : class;
        object TryResolve(Type type);
    }
}