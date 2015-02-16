using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Client.Logic;
using Common.DataContract;

namespace Client.Model
{
    public class ClientModel
    {
        #region Properties

        public static Player Player;

        public static string MapName = ConfigurationManager.AppSettings["MapName"];

        private static ClientProcessor _clientProcessor = new ClientProcessor();

        public string Message;

        #endregion Properties

        #region Services Methods

        public static void RegisterMe(string login)
        {
            Proxy.Instance.RegisterMe(login);
        }

        public static void StartGame()
        {
            Proxy.Instance.StartGame(Path.GetFileName(MapName));
        }

        public static void PlayerAction(ActionType actionType)
        {
            Proxy.Instance.PlayerAction(actionType);
        }

        #endregion Services Methods

        #region Callback Services Methods

        public void OnConnection(Player mePlayer, List<string> logins)
        {
            //Register myself
            Player = mePlayer;
            //warn viewmodel to change the wiew with the list of player connected
            _clientProcessor.OnConnected(Player, logins, Player.IsCreator);
        }

        public void OnUserConnected(List<String> logins)
        {
            _clientProcessor.OnUserConnected(logins);
        }

        public void OnGameStarted(Game newGame)
        {
            //initialize Bomb power
            Player.BombPower = 1;
            //send the new map to the view model 
            _clientProcessor.OnGameStarted(newGame);
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            if (Player.ID == player.ID)
                Player.Position = newPosition;

            _clientProcessor.OnPlayerMove(player, newPosition, actionType);
        }

        public void OnBombDropped(Bomb newBomb)
        {
            _clientProcessor.OnBombDropped(newBomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            _clientProcessor.OnBombExploded(bomb, impacted);
        }

        public void OnPlayerDeath(Player player)
        {
            Message = player.Username + " is dead. GOGO for the win !!";
            _clientProcessor.DisplayMessage(Message);
            _clientProcessor.OnPlayerDeath(player);
        }

        public void OnMyDeath()
        {
            Message = Player.IsCreator ? "You are dead. Please wait for the end to restart the game" : "You are dead. Please wait for the creator to restart the game";
            _clientProcessor.DisplayMessage(Message);
            _clientProcessor.OnMyDeath();
        }

        public void OnDraw()
        {
            Message = "Everybody lost ... :/";
            _clientProcessor.DisplayMessage(Message);
        }

        public void OnWin()
        {
            Message = "You are the winner GJ !!.";
            _clientProcessor.DisplayMessage(Message);
        }

        public void OnCanRestartGame()
        {
            // TODO
        }

        #endregion Callback Services Methods
    }
}
