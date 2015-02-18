
using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class UserManager : IUserManager
    {
        private readonly List<User> _users;

        private int _playerCounter;

        public UserManager()
        {
            _users = new List<User>();
        }

        public User CreateNewUser(string username, IBombermanCallbackService callback)
        {
            if (string.IsNullOrEmpty(username) || callback == null || IsUsernameAlreadyUsed(username))
            {
                Log.WriteLine(Log.LogLevels.Error, "problem with username or callback");
                return null;
            }
            //create user
            User newUser = new User
            {
                ID = Guid.NewGuid(),
                CallbackService = callback,
                Status = UserStatus.Connected,
                Player = new Player
                {
                    Alive = true,
                    BombNumber = 1,
                    BombPower = 1,
                    CanShootBomb = false,
                    ID = _playerCounter++,
                    Score = 0,
                    Username = username,
                }
            };
            //add user to collection
            _users.Add(newUser);
            //return new user
            return newUser;
        }

        public void DeleteUser(User user)
        {
            if (user == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem with user");
                return;
            }
            if (_users == null || !_users.Any() || !_users.Contains(user))
            {
                Log.WriteLine(Log.LogLevels.Error, "User not registered");
                return;
            }
            _users.Remove(user);
        }

        public User GetUserById(Guid id)
        {
            if (_users == null || !_users.Any())
                return null;
                
            return _users.FirstOrDefault(x => x.ID == id);
        }

        public User GetUserByCallback(IBombermanCallbackService callback)
        {
            if (callback == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "problem with callback");
                return null;
            }

            if (_users == null || !_users.Any())
            {
                return null;
            }
                
            return _users.FirstOrDefault(x => x.CallbackService == callback);
        }

        public UserStatus GetUserStatusById(Guid id)
        {
            if (_users == null)
                return UserStatus.Unknown;
            User user = _users.FirstOrDefault(x => x.ID == id);
            return user == null ? UserStatus.Unknown : user.Status;
        }

        public IEnumerable<string> GetListOfUsername()
        {
            if (_users == null || !_users.Any())
                return null;
            return _users.Select(x => x.Player.Username).ToList();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users;
        }

        public IEnumerable<User> GetAllOtherUsers(User user)
        {
            if (user == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "problem with user");
                return null;
            }

            if (_users == null)
                return null;

            return _users.Where(x => x.ID != user.ID).ToList();
        }

        public bool IsUsernameAlreadyUsed(string username)
        {
            if (_users == null || !_users.Any() || string.IsNullOrEmpty(username))
                return false;
            return _users.Any(x => x.Player.Username == username);
        }
    }
}
