using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using ClientWPF.Proxies;
using ClientWPF.ViewModels;
using ClientWPF.ViewModels.StartedGame;
using Common.DataContract;

namespace ClientWPF.Logic
{
    public class ClientModel
    {

        #region Properties

        public Player Player { get; set; }

        public Map Map { get; set; }

        public static string MapPath = ConfigurationManager.AppSettings["MapPath"];

        public BombermanViewModel BombermanViewModel { get; set; }

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
            BombermanViewModel.OnGameStarted(newGame);
        }

        public void OnPlayerMove(Player player, Position newPosition)
        {
            if (Player.CompareId(player))
                Player.ObjectPosition = newPosition;

            BombermanViewModel.OnPlayerMove(player, newPosition);
            
        }
        #endregion Callback Services Methods



       
    }
}
