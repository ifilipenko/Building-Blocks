using System.ServiceModel;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Wcf.Helpers
{
    public static class InjectionHelper
    {
        public static void ResolveWithProxyFor<TContract>(
            this IocContainer iocContainer, 
            string endpointName = null)
        {
            iocContainer.ResolveBy(() => ProxyFor<TContract>(endpointName ?? typeof (TContract).Name));
        }

        private static TContract ProxyFor<TContract>(string endpointConfigurationName)
        {
            var factory = new ChannelFactory<TContract>(endpointConfigurationName);
            var proxy = factory.CreateChannel();
            return proxy;
        }
    }
}