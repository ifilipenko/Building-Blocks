using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Wcf
{
    public class WcfProxyFactory
    {
        private Binding _binding;

        public WcfProxyFactory()
        {
            _binding = new BasicHttpBinding();
        }

        public Binding Binding
        {
            get { return _binding; }
            set
            {
                Condition.Requires(value, "Binding").IsNotNull();
                _binding = value;
            }
        }

        public string BaseUrl { get; set; }

        public TContract CreateProxy<TContract>(string url)
        {
            Condition.Requires(url, "url").IsNotNullOrEmpty();

            var endpointUri = new Uri(BaseUrl == null ? url : BaseUrl + url);
            var endPointAddress = new EndpointAddress(endpointUri.ToString());
            var factory = new ChannelFactory<TContract>(Binding, endPointAddress);
            var proxy = factory.CreateChannel();
            return proxy;
        }
    }
}