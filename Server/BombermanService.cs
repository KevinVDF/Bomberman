using System;
using System.ServiceModel;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanService : IBombermanService
    {
        public Server Server { get; private set; }

        public BombermanService(Server server)
        {
            Server = server;
            Uri baseAddress = new Uri("net.tcp://localhost:7900");

            ServiceHost serviceHost = new ServiceHost(this, baseAddress);
            serviceHost.AddServiceEndpoint(typeof(IBombermanService), new NetTcpBinding(SecurityMode.None), "/BombermanCallbackService");
            serviceHost.Open();

            foreach (var endpt in serviceHost.Description.Endpoints)
            {
                Log.WriteLine(Log.LogLevels.Debug, "Enpoint address:\t{0}", endpt.Address);
                Log.WriteLine(Log.LogLevels.Debug, "Enpoint binding:\t{0}", endpt.Binding);
                Log.WriteLine(Log.LogLevels.Debug, "Enpoint contract:\t{0}\n", endpt.Contract.ContractType.Name);
            }
        }

        public void RegisterMe(string username)
        {
            IBombermanCallbackService callback = OperationContext.Current.GetCallbackChannel<IBombermanCallbackService>();
            Log.WriteLine(Log.LogLevels.Debug, "RegisterMe:{0}", username);
            Server.ConnectUser(callback, username);
        }

        public void LeaveGame(Guid ID)
        {
            Log.WriteLine(Log.LogLevels.Debug, "LeaveGame:{0}", ID);
            Server.DisconnectUser(ID);
        }

        public void StartGame()
        {
            Log.WriteLine(Log.LogLevels.Debug, "StartGame");
            Server.StartNewGame();
        }

        public void PlayerAction(Guid ID, ActionType actionType)
        {
            Log.WriteLine(Log.LogLevels.Debug, "PlayerAction :{0}", actionType.ToString());
            Server.PlayerAction(ID, actionType);
        }
    }
}
