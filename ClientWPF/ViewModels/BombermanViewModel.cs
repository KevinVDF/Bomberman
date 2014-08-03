<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using ClientWPF.CallBackService;
using ClientWPF.Logic;
using ClientWPF.ViewModels.GameRoom;
using ClientWPF.ViewModels.Login;
using Common.DataContract;
using Common.Interfaces;
using System.ServiceModel;
=======
﻿using ClientWPF.ViewModels.Login;
using Common.Interfaces;
>>>>>>> origin/master

namespace ClientWPF.ViewModels
{
    public class BombermanViewModel : ViewModelBase
    {
        #region Properties

<<<<<<< HEAD
        public Player Player { get; set; }
=======
        public IBombermanService Proxy { get; private set; }
>>>>>>> origin/master

        private LoginViewModel _loginViewModel;
        public LoginViewModel LoginViewModel
        {
            get { return _loginViewModel; }
            set { Set(() => LoginViewModel, ref _loginViewModel, value); }
        }

<<<<<<< HEAD
        private GameRoomViewModel _gameRoomViewModel;
        public GameRoomViewModel GameRoomViewModel
        {
            get { return _gameRoomViewModel; }
            set { Set(() => GameRoomViewModel, ref _gameRoomViewModel, value); }
        }

        public BombermanViewModel()
        {
            LoginViewModel = new LoginViewModel();
            GameRoomViewModel = new GameRoomViewModel();
        }

=======
>>>>>>> origin/master
        #endregion 

        #region Methods

        public void Initialize()
        {
<<<<<<< HEAD
            var context = new InstanceContext(new BombermanCallbackService(this));
            var factory = new DuplexChannelFactory<IBombermanService>(context, "WSDualHttpBinding_IBombermanService");
            LoginViewModel.Initialize(true,factory.CreateChannel());
            GameRoomViewModel.Initialize(false,"");
        }

        public void OnUserConnected(Player newPlayer, List<String> loginsList, bool canStartGame)
        {
            if(Player == null)
                Player = newPlayer;
            string richText = "";
            richText += "--------------------------------------\n";
            richText += "-------- Welcome to Bomberman -----\n";
            richText += "-----------" + Player.Username +"------------\n\n";
            richText += "New User Joined the server : " + newPlayer.Username + "\n";
            richText += "List of players online :\n\n";
            richText = loginsList.Aggregate(richText, (current, login) => current + (login + "\n\n"));
            if (Player.IsCreator)
            {
                //todo don't allow user to click on s if canstartgame is false
                richText += canStartGame ? "Press S to start the game" : "Wait for other players.";
            }
            else richText += "Wait until the creator start the game.";
            GameRoomViewModel.RichText = richText;
            LoginViewModel.IsVisible = false;
            GameRoomViewModel.IsVisible = true;
            if (canStartGame)
                GameRoomViewModel.IsStartEnabled = true;
            GameRoomViewModel.IsStartVisible = Player.IsCreator;
=======
            LoginViewModel.Initialize(Proxy);
>>>>>>> origin/master
        }

        #endregion
    }

    public class BombermanViewModelDesignData : BombermanViewModel
    {
        public BombermanViewModelDesignData()
        {
            LoginViewModel = new LoginViewModelDesignData();
<<<<<<< HEAD
            GameRoomViewModel = new GameRoomViewModelDesignData();
=======
>>>>>>> origin/master
        }
    }
}
