using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using Common.DataContract;
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

        public void SendError(IBombermanCallbackService callback, string errorMessage, ErrorType errorType)
        {
            callback.OnError(errorMessage, errorType);
        }

        public void SendUsernameListToNewUser(User newUser)
        {
            if (newUser == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "User unknown");
                return;
            }

            IEnumerable<string> usernames = _userManager.GetListOfUsername();

            if (usernames == null || !usernames.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting listOfUsername");
                return;
            }

            Log.WriteLine(Log.LogLevels.Info, "usernames List Send to new user");

            ExceptionFreeAction(newUser, user => user.CallbackService.OnConnection(newUser.ID, usernames));
        }

        public void SendUsernameListToAllOtherUserAfterConnection(User newUser)
        {
            if (newUser == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "User unknown");
                return;
            }

            IEnumerable<User> otherUsers = _userManager.GetAllOtherUsers(newUser);

            if (otherUsers == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting AllOtherUser list");
                return;
            }

            if (!otherUsers.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Not supposed to be called");
                return;
            }

            IEnumerable<string> usernames = _userManager.GetListOfUsername();

            if (usernames == null || !usernames.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting listOfUsername");
                return;
            }

            Log.WriteLine(Log.LogLevels.Info, "usernames List Send to other users after connection");

            ExceptionFreeAction(otherUsers, otherPlayer => otherPlayer.CallbackService.OnUserConnected(usernames));
        }

        public void SendUsernameListToAllOtherUserAfterDisconnection(User user)
        {
            if (user == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "User unknown");
                return;
            }

            IEnumerable<User> otherUsers = _userManager.GetAllOtherUsers(user);

            if (otherUsers == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting AllOtherUser list");
                return;
            }

            if (!otherUsers.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Not supposed to be called");
                return;
            }

            IEnumerable<string> usernames = _userManager.GetListOfUsername();

            if (usernames == null || !usernames.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting listOfUsername");
                return;
            }

            Log.WriteLine(Log.LogLevels.Info, "usernames List Send to other users after disconnection");

            ExceptionFreeAction(otherUsers, otherPlayer => otherPlayer.CallbackService.OnUserDisconnected(usernames));
        }

        public void SendGameToAllUsers(Game newGame)
        {
            if (newGame == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "game unknown");
                return;
            }

            IEnumerable<User> users = _userManager.GetAllUsers();

            if (users == null || !users.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting all users");
                return;
            }

            Log.WriteLine(Log.LogLevels.Info, "game send to all users");

            ExceptionFreeAction(users, user => user.CallbackService.OnGameStarted(user.Player, newGame));
        }

        public void SendMoveToAllUsers(User userMoved, Position newPosition)
        {
            if (userMoved == null || newPosition == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "unknown user to move / new Position");
                return;
            }

            IEnumerable<User> users = _userManager.GetAllUsers();

            if (users == null || !users.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting all users");
                return;
            }

            Log.WriteLine(Log.LogLevels.Info, "Move succeed : send new position to all user");

            ExceptionFreeAction(users, user => user.CallbackService.OnPlayerMove(userMoved.Player, newPosition));

        }

        private void ExceptionFreeAction(User user, Action<User> action)
        {
            try
            {
                Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeSingle : {0} : {1} ", user.Username, action.Method.Name);
                action(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error with player " + user.Username);
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
                    Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeList : {0} : {1} ", user.Username, action.Method.Name);
                    action(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + user.Username);
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
