using System;
using System.Collections.Generic;
using System.Linq;
using Common.Log;
using Server.Model;

namespace Server.Manager
{
    public class CallBackManager
    {
        public static List<UserModel> PlayersOnline { get; set; }

        public static void SendUsernameListToNewPlayer(UserModel newPlayer)
        {
            ExceptionFreeAction(newPlayer, player => newPlayer.CallbackService.OnConnection(newPlayer.Player, PlayersOnline.Select(x => x.Player.Username).ToList()));
        }

        public static void OnUserConnected(UserModel user)
        {
            ExceptionFreeAction(PlayersOnline.Where(player => player != newPlayer), otherPlayer => otherPlayer.CallbackService.OnUserConnected(playersNamesList));
        }

        private static void ExceptionFreeAction(UserModel player, Action<UserModel> action)
        {
            try
            {
                Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeSingle : {0} : {1} ", player.Player.Username, action.Method.Name);
                action(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error with player " + player.Player.Username);
                Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                PlayersOnline.Remove(player);
            }
        }
        //OKAY
        private static void ExceptionFreeAction(IEnumerable<UserModel> players, Action<UserModel> action)
        {
            List<UserModel> disconnected = new List<UserModel>();
            var playerModels = players as UserModel[] ?? players.ToArray();
            if (players == null || !playerModels.Any())
                return;
            foreach (UserModel player in playerModels)
            {
                try
                {
                    Log.WriteLine(Log.LogLevels.Debug, "ExceptionfreeList : {0} : {1} ", player.Player.Username, action.Method.Name);
                    action(player);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + player.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    disconnected.Add(player);
                }
            }
            foreach (UserModel playerDisconnected in disconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
            }
        }
    }
}
