using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
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
            Binding binding = new NetTcpBinding(SecurityMode.None);


            InstanceContext instanceContext = new InstanceContext(new BombermanCallbackService(bombermanViewModel));//WP0483
            DuplexChannelFactory<IBombermanService> factory = new DuplexChannelFactory<IBombermanService>(instanceContext, binding, new EndpointAddress(
                new Uri(string.Concat("net.tcp://",ConfigurationManager.AppSettings["MachineName"],":7900/BombermanCallbackService"))));
            _instance = factory.CreateChannel();
        }
    }
}
