using System.ServiceModel;
using Common.DataContract;
using Common.Interfaces;
using Server.Model;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanService : IBombermanService
    {
        //public ServerModel Server = new ServerModel();
        public ServerModel Server { get; private set; }

        private readonly ServiceHost _serviceHost;

        public BombermanService(ServerModel server)
        {
            Server = server;

            _serviceHost = new ServiceHost(this);
            _serviceHost.Open();
        }


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

        public void RestartGame()
        {
            Server.RestartGame();
        }
    }
}
