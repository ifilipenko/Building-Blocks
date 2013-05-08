using System;

namespace BuildingBlocks.Common
{
    public interface IIocRegistration
    {
        void Register<T>(Func<IIocContainer, T> factoryMethod, Lifecycle lifecycle = Lifecycle.AlwaysNew);
        void Register(Type type, Func<IIocContainer, object> factoryMethod, Lifecycle lifecycle = Lifecycle.AlwaysNew);
    }
}