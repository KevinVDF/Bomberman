using System;
using System.Linq;
using System.Text;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;
using Server.Manager;
using Server.Manager.Interface;

namespace Server
{
    class Program
    {
        static void Main()
        {
            Console.SetWindowSize(80, 50);

            Log.Initialize(@"D:\Temp\BombermanLogs","Server.log");

            IUserManager userManager = new UserManager();
            ICallbackManager callbackManager= new CallbackManager(userManager);
            IMapManager mapManager = new MapManager(userManager);
            IGameManager gameManager = new GameManager();

            Server server = new Server(userManager,callbackManager,gameManager,mapManager);

            IBombermanService service = new BombermanService(server);

            Log.WriteLine(Log.LogLevels.Info, "Server Started at " + DateTime.Now.ToShortTimeString());

            DumpHelp();

            bool stop = false;
            while (!stop)
            {
                if (!Console.KeyAvailable) 
                    continue;
                ConsoleKeyInfo read = Console.ReadKey(true);
                switch (read.Key)
                {
                    case ConsoleKey.M:
                        //if (server.GameCreated != null && server.GameCreated.Map != null)
                        //    DumpMap(server.GameCreated.Map);
                        //else
                            Console.WriteLine("No map");
                        break;
                    case ConsoleKey.P:
                        DumpPlayers(server);
                        break;
                    case ConsoleKey.X:
                        stop = true;
                        break;
                    default:
                        DumpHelp();
                        break;
                }
            }
        }

        static void DumpHelp()
        {
            Console.WriteLine("X: quit");
            Console.WriteLine("M: dump map");
            Console.WriteLine("P: dump player list");
        }

        static void DumpPlayers(Server server)
        {
            Console.WriteLine("Connected: {0}", server.UserManager.GetAllUsers().Count());
            foreach (User user in server.UserManager.GetAllUsers())
                Console.WriteLine("{0}:{1} alive:{2} maxbomb:{3}", user.Player.ID, user.Username, user.Player.Alive, user.Player.BombNumber);
            //Console.WriteLine("Disconnected: {0}", server.PlayersDisconnected.Count);
            //foreach (PlayerModel player in server.PlayersDisconnected)
            //    Console.WriteLine("{0}:{1} alive:{2} iscreator:{3} maxbomb:{4}", player.Player.Id, player.Player.Username, player.Alife, player.Player.IsCreator, player.Player.MaxBombCount);
        }

        static void DumpMap(Map map)
        {
            for(int y = 0; y < map.MapSize; y++) {
                StringBuilder line = new StringBuilder(map.MapSize);
                for (int x = 0; x < map.MapSize; x++)
                {
                    // search object at this position
                    LivingObject item = map.LivingObjects.FirstOrDefault(o => o.Position.X == x && o.Position.Y == y);
                    char c = MapLivingObjectToChar(item);
                    line.Append(c);
                }
                Console.WriteLine(line);
            }
        }

        static char MapLivingObjectToChar(LivingObject item)
        {
            if (item == null)
                return ' ';
            if (item is Wall)
            {
                Wall wall = item as Wall;
                return wall.WallType == WallType.Undestructible ? '█' : '.';
            }
            if (item is Player)
                return 'x';
            if (item is Bomb)
                return '*';
            if (item is Bonus)
                return '=';
            return '?';
        }
    }
}
