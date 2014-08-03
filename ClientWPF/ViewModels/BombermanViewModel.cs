using System;
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

namespace ClientWPF.ViewModels
{
    public class BombermanViewModel : ViewModelBase
    {
        #region Properties

        public Player Player { get; set; }

        private LoginViewModel _loginViewModel;
        public LoginViewModel LoginViewModel
        {
            get { return _loginViewModel; }
            set { Set(() => LoginViewModel, ref _loginViewModel, value); }
        }

        private GameRoomViewModel _gameRoomViewModel;
        public GameRoomViewModel GameRoomViewModel
        {
            get { return _gameRoomViewModel; }
            set { Set(() => GameRoomViewModel, ref _gameRoomViewModel, value); }
        }

        private StartedGameViewModel _startedGameViewModel;
        public StartedGameViewModel StartedGameViewModel
        {
            get { return _startedGameViewModel; }
            set { Set(() => StartedGameViewModel, ref _startedGameViewModel, value); }
        }

        #endregion 

        #region Methods

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
        }

        public void OnUserConnected(Player newPlayer, List<String> loginsList, bool canStartGame)
        {
            // if the player just connected then initialize it 
            if (Player == null)
            {
                Player = newPlayer;
                GameRoomViewModel.Player = newPlayer;
            }
            // generate text in the room game
            GameRoomViewModel.GenerateGameRoomText(newPlayer,loginsList, canStartGame) ;
            //pass to game room mode
            LoginViewModel.IsVisible = false;
            GameRoomViewModel.IsVisible = true;
            if (canStartGame)
                GameRoomViewModel.IsStartEnabled = true;
            //check if the player is creator then he has the button visible
            GameRoomViewModel.IsStartVisible = Player.IsCreator;
        }

        public void OnGameStarted(Game newGame)
        {
            GameRoomViewModel.IsVisible = false;
            StartedGameViewModel.IsVisible = true;
            StartedGameViewModel.Game = newGame;
        }

        #endregion
    }

    public class BombermanViewModelDesignData : BombermanViewModel
    {
        public BombermanViewModelDesignData()
        {
            LoginViewModel = new LoginViewModelDesignData();
            GameRoomViewModel = new GameRoomViewModelDesignData();
        }
    }
}
