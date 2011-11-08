using System;
using Common.Logging;
using CuttingEdge.Conditions;
using StructureMap;

namespace BuildingBlocks.Configuration
{
    public class IocContainer
    {
        private static readonly ILog _log = LogManager.GetLogger<IocContainer>();
        private readonly ConfigurationExpression _iocContainerConfiguration;

        public IocContainer(ConfigurationExpression iocContainerConfiguration)
        {
            Condition.Requires(iocContainerConfiguration, "iocContainerConfiguration").IsNotNull();

            _iocContainerConfiguration = iocContainerConfiguration;
        }

        public ConfigurationExpression IocContainerConfiguration
        {
            get { return _iocContainerConfiguration; }
        }

        public void ResolveBy<T>(Func<T> func)
        {
            _iocContainerConfiguration
                .For<T>()
                .Use(func);
        }

        public void SelfResolve(object obj)
        {
            Condition.Requires(obj, "obj").IsNotNull();

            _iocContainerConfiguration
                .For(obj.GetType())
                .Singleton()
                .Use(obj);
        }

        public void ResolveWithInstance<T>(T obj)
        {
            _iocContainerConfiguration
                .For<T>()
                .HybridHttpOrThreadLocalScoped()
                .Use(obj);
        }

        public ResolveExpression<T> Resolve<T>()
        {
            return new ResolveExpression<T>(_iocContainerConfiguration);
        }

        public InterfacesImplemenationScanner ForInterfacesAll(Action<InterfacesSelector> interfacesSelector)
        {
            return new InterfacesImplemenationScanner(interfacesSelector, _iocContainerConfiguration);
        }
    }
}