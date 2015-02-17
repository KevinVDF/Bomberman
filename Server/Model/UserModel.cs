using System;
using Common.DataContract;
using Common.Interfaces;

namespace Server.Model
{
    public class UserModel
    {
        public Guid ID { get; set; }

        public Player Player { get; set; }

        public IBombermanCallbackService CallbackService { get; set; } 

        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Connected,
        Disconnected
    }
}
