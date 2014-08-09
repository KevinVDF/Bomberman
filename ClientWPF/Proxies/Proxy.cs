using System.ServiceModel;
using ClientWPF.CallBackService;
using ClientWPF.ViewModels;
using Common.Interfaces;

namespace ClientWPF.Proxies
{
    public class Proxy
    {
        
        private static IBombermanService _instance;

        private Proxy(){}

        public static IBombermanService Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void SetViewModel(BombermanViewModel bombermanViewModel)
        {
            var context = new InstanceContext(new BombermanCallbackService(bombermanViewModel));
            var factory = new DuplexChannelFactory<IBombermanService>(context, "netTcpBinding_IBombermanService");
            _instance = factory.CreateChannel();
        }
    }
}
