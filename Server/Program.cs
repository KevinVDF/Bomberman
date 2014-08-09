using System;
using System.ServiceModel;
using Common.Log;

namespace Server
{
    class Program
    {
        static void Main()
        {
            Log.Initialize(@"D:\Temp\BombermanLogs","Server.log");
            Log.WriteLine(Log.LogLevels.Info,  "Server Started at " + DateTime.Now.ToShortTimeString());
            var svcHost = new ServiceHost(typeof (BombermanService));
            svcHost.Open();
            bool stop = false;
            while (!stop)
            {
                var read = Console.ReadKey();
                switch (read.KeyChar)
                {
                    case '1':
                        break;
                    case '2':
                        break;
                    default:
                        stop = true;
                        break;
                }
            }
            svcHost.Close();
        } 
    }
}
