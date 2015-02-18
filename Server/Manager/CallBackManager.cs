using System;
using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using Common.Log;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class CallbackManager : ICallbackManager
    {
        private readonly IUserManager _userManager;

        public CallbackManager(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void SendErrorOnConnection(IBombermanCallbackService callback, string errorMessage)
        {
            callback.OnErrorConnection(errorMessage);
        }

        public void SendUsernameListToNewPlayer(User newUser, IEnumerable<String> usernames)
        {
            ExceptionFreeAction(newUser, player => newUser.CallbackService.OnConnection(newUser.Player, usernames));
        }

        public void SendUsernameListToAllOtherUser(IEnumerable<User> otherUsers, IEnumerable<String> usernames)
        {
            ExceptionFreeAction(otherUsers, otherPlayer => otherPlayer.CallbackService.OnUserConnected(usernames));
        }

        private void ExceptionFreeAction(User user, Action<User> action)
        {
            try
            {
                Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeSingle : {0} : {1} ", user.Player.Username, action.Method.Name);
                action(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error with player " + user.Player.Username);
                Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                _userManager.DeleteUser(user);
            }
        }

        private void ExceptionFreeAction(IEnumerable<User> users, Action<User> action)
        {
            List<User> disconnected = new List<User>();
            if (users == null)
                return;
            foreach (User user in users)
            {
                try
                {
                    Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeList : {0} : {1} ", user.Player.Username, action.Method.Name);
                    action(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + user.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    disconnected.Add(user);
                }
            }
            foreach (User playerDisconnected in disconnected)
            {
                _userManager.DeleteUser(playerDisconnected);
            }
        }
    }
}
