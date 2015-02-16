using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace Client
{
    //TODO !! var/params names
    class Program
    {

        public const string MapPath = @"F:\\Bomberman\Server\map.dat";

        static void Main()
        {
            string username = "Console_"+Guid.NewGuid().ToString().Substring(0,5);

            Log.Initialize(@"D:\Temp\BombermanLogs", "Client_" + username + ".log");
            Log.WriteLine(Log.LogLevels.Info, "Logged at " + DateTime.Now.ToShortTimeString());

            //
            Binding binding = new NetTcpBinding(SecurityMode.None);
            InstanceContext instanceContext = new InstanceContext(new BombermanCallbackService());//WP0483
            DuplexChannelFactory<IBombermanService> factory = new DuplexChannelFactory<IBombermanService>(instanceContext, binding, new EndpointAddress(
                new Uri(string.Concat("net.tcp://", ConfigurationManager.AppSettings["MachineName"], ":7900/BombermanCallbackService"))));
            Proxy.Instance = factory.CreateChannel();
           
            //
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("--------------------------------------\n\n");
            //Console.WriteLine("Type your player name :\n");
            //string login = Console.ReadLine();
            //username = login;
            ConnectPlayer(username);
            //Log.Initialize(@"D:\Temp\BombermanLogs", "Client_" + login +".log");
            //Log.WriteLine(Log.LogLevels.Info, "Logged at " + DateTime.Now.ToShortTimeString());

            bool stop = false;
            while (!stop)
            {
                ConsoleKeyInfo keyboard = Console.ReadKey();
                switch (keyboard.Key)
                {
                    //s
                    case ConsoleKey.S:
                        StartGame();
                        break;
                    case ConsoleKey.UpArrow:
                        MoveTo(ActionType.MoveUp);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveTo(ActionType.MoveLeft);
                        break;
                    case ConsoleKey.RightArrow:
                        MoveTo(ActionType.MoveRight);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveTo(ActionType.MoveDown);
                        break;
                    case ConsoleKey.Spacebar:
                        DropBomb();
                        break;
                    case ConsoleKey.X: // SinaC: never leave a while(true) without an exit condition
                        stop = true;
                        break;
                }
            }
        }

        //todo replace playername by an id ...
        private static void ConnectPlayer(string username)
        {
            Proxy.Instance.RegisterMe(username);
        }

        private static void StartGame()
        {
            Proxy.Instance.StartGame("TEST.DAT");
        }

        private static void MoveTo(ActionType actionType)
        {
            Proxy.Instance.PlayerAction(actionType);
        }

        private static void DropBomb()
        {
            Proxy.Instance.PlayerAction(ActionType.DropBomb);
        }
    }
}
