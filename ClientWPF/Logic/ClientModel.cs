using System;
using System.Collections.Generic;
using System.Configuration;
using ClientWPF.Proxies;
using ClientWPF.ViewModels;
using Common.DataContract;

namespace ClientWPF.Logic
{
    public class ClientModel
    {

        #region Properties

        public static Player Player;

        public static string MapPath = ConfigurationManager.AppSettings["MapPath"];

        public static  BombermanViewModel BombermanViewModel;

        public string Message;


        #endregion Properties

        public ClientModel(BombermanViewModel bombermanViewModel)
        {
            BombermanViewModel = bombermanViewModel;
        }

        #region Services Methods

        public static void RegisterMe(string login)
        {
            Proxy.Instance.RegisterMe(login);
        }

        public static void StartGame()
        {
            Proxy.Instance.StartGame(MapPath);
        }

        public static void PlayerAction(ActionType actionType)
        {
            Proxy.Instance.PlayerAction(actionType);
        }

        public static void RestartGame()
        {
            Proxy.Instance.RestartGame();
        }

        #endregion Services Methods

        #region Callback Services Methods

        public void OnConnection(Player mePlayer, List<string> logins)
        {
            //Register myself
            Player = mePlayer;
            //warn viewmodel to change the wiew with the list of player connected
            BombermanViewModel.OnConnection(Player.Username, logins, Player.IsCreator);
        }

        public void OnUserConnected(List<String> logins)
        {
            BombermanViewModel.OnUserConnected(Player.Username, logins , Player.IsCreator);
        }

        public void OnGameStarted(Game newGame)
        {
            //initialize Bomb power
            Player.BombPower = 1;
            //send the new map to the view model 
            BombermanViewModel.OnGameStarted(newGame);
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            if (Player.CompareId(player))
                Player.Position = newPosition;

            BombermanViewModel.OnPlayerMove(player, newPosition, actionType);
        }

        public void OnBombDropped(Bomb newBomb)
        {
            BombermanViewModel.OnBombDropped(newBomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            BombermanViewModel.OnBombExploded(bomb, impacted);
        }

        public void OnPlayerDeath(Player player)
        {
            Message = player.Username + " is dead. GOGO for the win !!";
            BombermanViewModel.DisplayMessage(Message);
        }

        public void OnMyDeath()
        {
            Message = Player.IsCreator ? "You are dead. Please wait for the end to restart the game" : "You are dead. Please wait for the creator to restart the game";
            BombermanViewModel.DisplayMessage(Message);
        }

        public void OnDraw()
        {
            Message = "Everybody lost ... :/";
            BombermanViewModel.DisplayMessage(Message);
        }

        public void OnWin()
        {
            Message = "You are the winner GJ !!.";
            BombermanViewModel.DisplayMessage(Message);
        }

        public void OnCanRestartGame()
        {
            BombermanViewModel.OnCanRestart();
        }

        #endregion Callback Services Methods
    }
}
