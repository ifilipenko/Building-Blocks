using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace BuildingBlocks.Wcf.Persistence
{
    public class NHibernateContextInitializer : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            instanceContext.Extensions.Add(new UnitOfWorkContext());
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var extensions = OperationContext.Current.InstanceContext.Extensions
                .FindAll<UnitOfWorkContext>();

            foreach (var extension in extensions)
            {
                OperationContext.Current.InstanceContext.Extensions.Remove(extension);
            }
        }
    }
}