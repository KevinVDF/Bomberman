﻿using System;
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
            //if first player online then warn wiewmodel that he can start a game
            bool canStartGame = logins.Count == 1;
            //warn viewmodel to change the wiew with the list of player connected
            BombermanViewModel.OnConnection(Player.Username, logins, canStartGame, Player.IsCreator);
        }

        public void OnUserConnected(List<String> loginsList)
        {
            //todo
            
        }

        public void OnGameStarted(Game newGame)
        {
            //todo
        }

        #endregion Properties

    }
}