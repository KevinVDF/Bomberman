using System;
using Common.DataContract;
using Common.Interfaces;

namespace Server
{
    public class User
    {
        public Guid ID { get; set; }

        public IBombermanCallbackService CallbackService { get; set; } 

        public UserStatus Status { get; set; }

        public Player Player { get; set; }

        public string Username{ get;set; }
 
    }

    public enum UserStatus
    {
        Unknown,
        ConnectionFaillure,
        Connected,
        Disconnected
    }
}
