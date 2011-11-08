using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace BuildingBlocks.Wcf
{
    [ServiceBehavior(InstanceContextMode =
      InstanceContextMode.Single,
      ConcurrencyMode = ConcurrencyMode.Multiple,
      AddressFilterMode = AddressFilterMode.Any,
      ValidateMustUnderstand = false)]
    public abstract class RouterService : IRouterService
    {
        public Message ProcessMessage(Message requestMessage)
        {
            string endpoint;
            BeforeSendMessage(ref requestMessage);
            ProcessRequestMessage(requestMessage, out endpoint);

            Message processMessage;
            using (var factory = new ChannelFactory<IRouterService>(endpoint))
            {
                factory.Endpoint.Behaviors.Add(new MustUnderstandBehavior(false));
                var proxy = factory.CreateChannel();

                // ReSharper disable SuspiciousTypeConversion.Global
                using (proxy as IDisposable)
                {
                    processMessage = proxy.ProcessMessage(requestMessage);
                }
                // ReSharper restore SuspiciousTypeConversion.Global
            }

            ProcessResponseMessage(requestMessage, processMessage);
            return processMessage;
        }

        protected abstract void ProcessResponseMessage(Message responseMessage, Message processMessage);
        protected abstract void ProcessRequestMessage(Message requestMessage, out string endpointName);

        protected virtual void BeforeSendMessage(ref Message message)
        {
        }

        protected XmlDocument CopyMessageToXml(ref Message message)
        {
            var buffer = message.CreateBufferedCopy(int.MaxValue);
            var messageCopy = buffer.CreateMessage();

            try
            {
                using (var reader = messageCopy.GetReaderAtBodyContents())
                {
                    var doc = new XmlDocument();
                    doc.Load(reader);
                    return doc;
                }
            }
            finally
            {
                message = buffer.CreateMessage();
            }
        }
    }
}