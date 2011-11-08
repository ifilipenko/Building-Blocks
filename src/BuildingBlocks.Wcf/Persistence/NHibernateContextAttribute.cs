using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Wcf.Persistence
{
    public class NHibernateContextAttribute : Attribute, IServiceBehavior
    {
        private readonly Type _configuratorStarterType;
        private ConfiguratorStarter _configuratorStarter;

        public NHibernateContextAttribute(Type configuratorStarterType)
        {
            _configuratorStarterType = configuratorStarterType;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, 
                                         ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (_configuratorStarter == null)
            {
                _configuratorStarter = (ConfiguratorStarter) Activator.CreateInstance(_configuratorStarterType);
            }
            _configuratorStarter.EnsureConfiguratorStarted();

            foreach (var endpoint in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany(d => d.Endpoints))
            {
                endpoint.DispatchRuntime.MessageInspectors.Add(new NHibernateContextInitializer());
            }
        }
    }
}
