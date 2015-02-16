using System;
using System.ServiceModel;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;
using Server.Model;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanService : IBombermanService
    {
        //public ServerModel Server = new ServerModel();
        public ServerModel Server { get; private set; }

        public BombermanService(ServerModel server)
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
            Log.WriteLine(Log.LogLevels.Debug, "Register Me:{0}", username);
            Server.ConnectUser(callback, username);
        }

        public void StartGame(string mapName)
        {
            Log.WriteLine(Log.LogLevels.Debug, "Start Game:{0}", mapName);
            Server.StartNewGame(mapName);
        }

        public void PlayerAction(ActionType actionType)
        {
            IBombermanCallbackService callback = OperationContext.Current.GetCallbackChannel<IBombermanCallbackService>();
            Log.WriteLine(Log.LogLevels.Debug, "Player Action :{0}", actionType.ToString());
            Server.PlayerAction(callback, actionType);
        }
    }
}
