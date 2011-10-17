using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace BuildingBlocks.Wcf
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior
    {
        private readonly IErrorHandler _errorHandler;
        
        public ErrorHandlerBehaviorAttribute(Type typeErrorHandler)
        {
            if (typeErrorHandler == null)
            {
                throw new ArgumentNullException();
            }
            _errorHandler = (IErrorHandler) Activator.CreateInstance(typeErrorHandler);
        }

        void IServiceBehavior.ApplyDispatchBehavior(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(_errorHandler);
            }
        }

        void IServiceBehavior.AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.Validate(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
        }
    }
}