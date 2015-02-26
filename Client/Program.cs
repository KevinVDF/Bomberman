using System;
using System.Configuration;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace Client
{
    //TODO !! var/params names
    class Program
    {
        public static IBombermanService Proxy { get; private set; }

        public static string Username { get; set; }

        public static bool ErrorConnection{ get; set; }

        public static ClientProcessor ClientProcessor { get; set; }

        [DllImport("Kernel32")]
        private static extern void SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate void EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static void Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    LeaveGame();
                    break;
                default:
                    LeaveGame();
                    break;
            }
        }

        static void Main()
        {
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            ClientProcessor = new ClientProcessor();

            var instanceContext = new InstanceContext(new BombermanCallbackService(ClientProcessor));
            Binding binding = new NetTcpBinding(SecurityMode.None);
            DuplexChannelFactory<IBombermanService> factory = new DuplexChannelFactory<IBombermanService>(instanceContext, binding, new EndpointAddress(
                new Uri(string.Concat("net.tcp://", ConfigurationManager.AppSettings["MachineName"], ":7900/BombermanCallbackService"))));
            Proxy = factory.CreateChannel();

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("--------------------------------------\n\n");

            do
            {
                Console.WriteLine("Type your player name :\n");
                Username = Console.ReadLine();
                ConnectUser(Username);
                ClientProcessor.Username = Username;
            } while (ErrorConnection);

            Log.Initialize(@"D:\Temp\BombermanLogs", "Client_" + Username + ".log");
            Log.WriteLine(Log.LogLevels.Info, "Logged at " + DateTime.Now.ToShortTimeString());

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
                    case ConsoleKey.X: // SinaC: never leave a while(true) without an exit condition
                        stop = true;
                        break;
                }
            }

            // SinaC: Clean properly factory
            try
            {
                factory.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Warning, "Exception:{0}", ex);
                factory.Abort();
            }
        }

        private static void ConnectUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ErrorConnection = true;
                return;
            }
                
            Proxy.RegisterMe(username);
        }

        private static void LeaveGame()
        {
            if (ClientProcessor.ID == Guid.Empty)
                return;
            Proxy.LeaveGame(ClientProcessor.ID);
        }


        private static void StartGame()
        {
            Proxy.StartGame();
        }

        private static void MoveTo(ActionType actionType)
        {
            Proxy.PlayerAction(ClientProcessor.ID, actionType);
        }

        
    }
}
