
using System;
using System.Collections.Generic;
using System.ServiceModel.Security;
using Common.Interfaces;

namespace Server.Manager.Interface
{
    public interface IUserManager
    {

        User CreateNewUser(string username, IBombermanCallbackService callback);

        void DeleteUser(User user);

        User GetUserById(Guid id);

        User GetUserByCallback(IBombermanCallbackService callback);

        UserStatus GetUserStatusById(Guid id);

        IEnumerable<string> GetListOfUsername();

        IEnumerable<User> GetAllOtherUsers(User user);

        IEnumerable<User> GetAllUsers();  

        bool IsUsernameAlreadyUsed(string username);
    }
}
