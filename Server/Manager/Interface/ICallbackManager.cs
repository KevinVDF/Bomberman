
using System;
using System.Collections.Generic;
using Common.Interfaces;

namespace Server.Manager.Interface
{
    public interface ICallbackManager
    {
        void SendErrorOnConnection(IBombermanCallbackService callback, string errorMessage);

        void SendUsernameListToNewPlayer(User newPlayer, IEnumerable<String> usernames);

        void SendUsernameListToAllOtherUser(IEnumerable<User> user, IEnumerable<String> usernames);
    }
}
