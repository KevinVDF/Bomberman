<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClientWPF.CallBackService;
using ClientWPF.ViewModels.GameRoom;
using ClientWPF.ViewModels.Login;
using ClientWPF.ViewModels.StartedGame;
using Common.DataContract;
using Common.Interfaces;
using System.ServiceModel;
=======
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
>>>>>>> origin/master

namespace ClientWPF.ViewModels
{
    public class BombermanViewModel : ViewModelBase
    {
        #region Properties

<<<<<<< HEAD
        public Player Player { get; set; }
=======
<<<<<<< HEAD
        public Player Player { get; set; }
=======
        public IBombermanService Proxy { get; private set; }
>>>>>>> origin/master
>>>>>>> origin/master

        private LoginViewModel _loginViewModel;
        public LoginViewModel LoginViewModel
        {
            get { return _loginViewModel; }
            set { Set(() => LoginViewModel, ref _loginViewModel, value); }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master
        private GameRoomViewModel _gameRoomViewModel;
        public GameRoomViewModel GameRoomViewModel
        {
            get { return _gameRoomViewModel; }
            set { Set(() => GameRoomViewModel, ref _gameRoomViewModel, value); }
        }

<<<<<<< HEAD
        private StartedGameViewModel _startedGameViewModel;
        public StartedGameViewModel StartedGameViewModel
        {
            get { return _startedGameViewModel; }
            set { Set(() => StartedGameViewModel, ref _startedGameViewModel, value); }
        }

=======
        public BombermanViewModel()
        {
            LoginViewModel = new LoginViewModel();
            GameRoomViewModel = new GameRoomViewModel();
        }

=======
>>>>>>> origin/master
>>>>>>> origin/master
        #endregion 

        #region Methods

<<<<<<< HEAD
        //Constructor
        public BombermanViewModel()
        {
            LoginViewModel = new LoginViewModel();
            GameRoomViewModel = new GameRoomViewModel();
            StartedGameViewModel = new StartedGameViewModel();
        }

        public void Initialize()
        {
            var context = new InstanceContext(new BombermanCallbackService(this));
            var factory = new DuplexChannelFactory<IBombermanService>(context, "WSDualHttpBinding_IBombermanService");
            var proxy = factory.CreateChannel();
            LoginViewModel.Initialize(true,proxy);
            GameRoomViewModel.Initialize(false,"", proxy );
            StartedGameViewModel.Initialize(false, proxy);
=======
        public void Initialize()
        {
<<<<<<< HEAD
            var context = new InstanceContext(new BombermanCallbackService(this));
            var factory = new DuplexChannelFactory<IBombermanService>(context, "WSDualHttpBinding_IBombermanService");
            LoginViewModel.Initialize(true,factory.CreateChannel());
            GameRoomViewModel.Initialize(false,"");
>>>>>>> origin/master
        }

        public void OnUserConnected(Player newPlayer, List<String> loginsList, bool canStartGame)
        {
<<<<<<< HEAD
            // if the player just connected then initialize it 
            if (Player == null)
            {
                Player = newPlayer;
                GameRoomViewModel.Player = newPlayer;
            }
            // generate text in the room game
            GameRoomViewModel.GenerateGameRoomText(newPlayer,loginsList, canStartGame) ;
            //pass to game room mode
=======
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
>>>>>>> origin/master
            LoginViewModel.IsVisible = false;
            GameRoomViewModel.IsVisible = true;
            if (canStartGame)
                GameRoomViewModel.IsStartEnabled = true;
<<<<<<< HEAD
            //check if the player is creator then he has the button visible
            GameRoomViewModel.IsStartVisible = Player.IsCreator;
        }

        public void OnGameStarted(Game newGame)
        {
            GameRoomViewModel.IsVisible = false;
            StartedGameViewModel.IsVisible = true;
            StartedGameViewModel.Game = newGame;
=======
            GameRoomViewModel.IsStartVisible = Player.IsCreator;
=======
            LoginViewModel.Initialize(Proxy);
>>>>>>> origin/master
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
<<<<<<< HEAD
            GameRoomViewModel = new GameRoomViewModelDesignData();
=======
>>>>>>> origin/master
>>>>>>> origin/master
        }
    }
}
