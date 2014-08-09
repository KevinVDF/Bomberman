using System;
using System.Collections.Generic;
using ClientWPF.ViewModels;
using Common.DataContract;

namespace ClientWPF.Logic
{
    public class ClientModel
    {

        #region Properties

        public Player Player { get; set; }

        public Map Map { get; set; }

        public BombermanViewModel BombermanViewModel { get; set; }

        #endregion Properties

        public ClientModel(BombermanViewModel bombermanViewModel)
        {
            BombermanViewModel = bombermanViewModel;
        }

        #region Methods

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
            //todo
        }

        #endregion Properties

    }
}
