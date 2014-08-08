using System;
using System.ServiceModel;
using Common.Log;
using Server.Model;

namespace Server
{
    class Program
    {
        public static ServerModel Server;


        static void Main()
        {
            Log.Initialize(@"D:\Temp\BombermanLogs","Server.log");

            Server = new ServerModel();
            Console.WriteLine("------ ServerStarted ------");
            Log.WriteLine(Log.LogLevels.Info,  "Server Started at " + DateTime.Now.ToShortTimeString());
            var svcHost = new ServiceHost(typeof (BombermanService));
            svcHost.Open();
            while (Server.ServerStatus == ServerStatus.Started)
            {
                Console.WriteLine("1) Players Online");
                Console.WriteLine("2) Server Status");
                var read = Console.ReadKey();
                switch (read.KeyChar)
                {
                    case '1':
                        GetOnlinePlayers();
                        break;
                    case '2':
                        Console.WriteLine("\n\nServer Status : " + Server.ServerStatus + "\n\n");
                        break;
                }
            }
            svcHost.Close();
        }

        private static void GetOnlinePlayers()
        {
            foreach (PlayerModel player in Server.PlayersOnline)
                Console.WriteLine("Player " + player.Player.Username + "Creator : " + player.Player.IsCreator);
        }
    }
}
