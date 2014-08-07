using System.ServiceModel;
using Common.DataContract;
using Server.Logic;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanService : Common.Interfaces.IBombermanService
    {
        private static readonly ServerProcessor ServerProcessor = new ServerProcessor();

        public static void StartServer()
        {
            ServerProcessor.StartServer();
        }

        public void ConnectUser(string username)
        {
            ServerProcessor.ConnectUser(username);
        }

        public void StartGame(string mapPath)
        {
            ServerProcessor.StartGame(mapPath);
        }

        public void MoveObjectToLocation(int idPlayer, ActionType actionType)
        {
            ServerProcessor.MoveObjectToLocation(idPlayer, actionType);
        }
    }
}
