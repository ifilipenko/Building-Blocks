using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace BuildingBlocks.Wcf
{
    public class EnsureAspNetCompatibilityServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost host = base.CreateServiceHost(serviceType, baseAddresses);
            var attrib = host.Description.Behaviors
                .Find<AspNetCompatibilityRequirementsAttribute>();
            if (attrib == null)
            {
                attrib = new AspNetCompatibilityRequirementsAttribute();
                host.Description.Behaviors.Add(attrib);
            }
            attrib.RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed;
            return host;
        }
    }
}