using System.ServiceModel;
using Common.DataContract;

namespace Common.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IBombermanCallbackService))]
    public interface IBombermanService
    {
        [OperationContract(IsOneWay = true)]
        void ConnectUser(string username);

        [OperationContract(IsOneWay = true)]
        void StartGame(string mapPath);

        [OperationContract(IsOneWay = true)]
        void MoveObjectToLocation(int idPlayer, ActionType actionType); //up,down,left,right
    }
}
