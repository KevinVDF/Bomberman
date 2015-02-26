
using System;
using System.Collections.Generic;
using Common.DataContract;
using Common.Interfaces;

namespace Server.Manager.Interface
{
    public interface ICallbackManager
    {
        void SendError(IBombermanCallbackService callback, string errorMessage, ErrorType errorType);

        void SendUsernameListToNewUser(User newPlayer);

        void SendUsernameListToAllOtherUserAfterConnection(User newUser);

        void SendUsernameListToAllOtherUserAfterDisconnection(User DisconnectedUser);

        void SendGameToAllUsers(Game newGame);
    }
}
