
using System;
using System.Collections.Generic;
using Common.DataContract;
using Common.Interfaces;

namespace Server.Manager.Interface
{
    public interface ICallbackManager
    {
        void SendError(IBombermanCallbackService callback, string errorMessage, ErrorType errorType);

        void SendUsernameListToNewPlayer(User newPlayer, IEnumerable<String> usernames);

        void SendUsernameListToAllOtherUserAfterConnection(IEnumerable<User> user, IEnumerable<String> usernames);

        void SendUsernameListToAllOtherUserAfterDisconnection(IEnumerable<User> user, IEnumerable<String> usernames);
    }
}
