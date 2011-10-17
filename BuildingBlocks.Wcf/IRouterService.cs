using System.ServiceModel;
using System.ServiceModel.Channels;

namespace BuildingBlocks.Wcf
{
    [ServiceContract]
    public interface IRouterService
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message ProcessMessage(Message requestMessage);
    }
}