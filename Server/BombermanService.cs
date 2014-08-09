using System.ServiceModel;
using Common.DataContract;
using Common.Interfaces;
using Server.Model;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanService : IBombermanService
    {
        public ServerModel Server = new ServerModel();

        public void RegisterMe(string username)
        {
            IBombermanCallbackService callback = OperationContext.Current.GetCallbackChannel<IBombermanCallbackService>();
            Server.ConnectUser(callback, username);
        }

        public void StartGame(string mapPath)
        {
            Server.StartGame(mapPath);
        }

        public void PlayerAction(ActionType actionType)
        {
            IBombermanCallbackService callback = OperationContext.Current.GetCallbackChannel<IBombermanCallbackService>();
            Server.PlayerAction(callback, actionType);
        }
    }
}
