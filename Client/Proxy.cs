using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.Interfaces;

namespace Client
{
    public class Proxy
    {
        public static IBombermanService Instance { get; set; }
    }
}
