using StructureMap;

namespace BuildingBlocks.Configuration
{
    public class ResolveExpression<T>
    {
        private readonly ConfigurationExpression _iocContainerConfiguration;

        public ResolveExpression(ConfigurationExpression iocContainerConfiguration)
        {
            _iocContainerConfiguration = iocContainerConfiguration;
        }

        public void WithType<TOut>()
            where TOut : T
        {
            _iocContainerConfiguration
                .For<T>()
                .HybridHttpOrThreadLocalScoped()
                .Use<TOut>();
        }
    }
}