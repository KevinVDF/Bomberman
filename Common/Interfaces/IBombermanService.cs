using System;
using System.ServiceModel;
using Common.DataContract;

namespace Common.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IBombermanCallbackService))]
    public interface IBombermanService
    {
        //on button login
        [OperationContract(IsOneWay = true)]
        void RegisterMe(string username);
        //on window closed
        [OperationContract(IsOneWay = true)] 
        void LeaveGame (Guid ID);
        //on button start
        [OperationContract(IsOneWay = true)]
        void StartGame(string mapName);
        //on any action
        [OperationContract(IsOneWay = true)]
        void PlayerAction(ActionType actionType);
    }
}
