using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientWPF.Logic;
using ClientWPF.Proxies;
using ClientWPF.ViewModels.GameRoom;
using ClientWPF.ViewModels.Login;
using ClientWPF.ViewModels.StartedGame;
using Common.DataContract;

namespace ClientWPF.ViewModels
{
    public class BombermanViewModel : ViewModelBase
    {
        #region Properties

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

        //Constructor
        public BombermanViewModel()
        {
            LoginViewModel = new LoginViewModel();
            GameRoomViewModel = new GameRoomViewModel();
            StartedGameViewModel = new StartedGameViewModel();
        }

        #region Methods

        public void Initialize()
        {
            LoginViewModel.Initialize(true);
            GameRoomViewModel.Initialize(false);
            StartedGameViewModel.Initialize(false);
            Proxy.SetViewModel(this);
        }

        public void OnConnection(string myUsername, List<string> logins, bool isCreator)
        {
            //pass to game room mode
            LoginViewModel.IsVisible = false;
            GameRoomViewModel.IsVisible = true;
            GameRoomViewModel.IsStartEnabled = false;
            //check if the player is creator then he can start a game
            GameRoomViewModel.IsStartVisible = isCreator;
            //warn the room that you join the server and if you can start a game
            GameRoomViewModel.GenerateTextOnConnection(myUsername, logins, isCreator);

            // TODO: remove
            GameRoomViewModel.IsStartVisible = true;
            GameRoomViewModel.IsStartEnabled = true;
        }

        public void OnUserConnected(string username, List<String> logins, bool isCreator)
        {
            //warn the room that you join the server and if you can start a game
            GameRoomViewModel.GenerateTextOnConnection(username, logins, isCreator);
        }

        public void OnGameStarted(Game newGame)
        {
            ////pass to started game mode
            GameRoomViewModel.IsVisible = false;
            StartedGameViewModel.IsVisible = true;
            ////set the new game retreive from server
            StartedGameViewModel.RegisterGame(newGame);
        }

        public void PlayerAction(ActionType actionType)
        {
            ClientModel.PlayerAction(actionType);
        }

        public void OnPlayerMove(Player player, Position newPosition)
        {
            StartedGameViewModel.OnPlayerMove(player, newPosition);
        }

        #endregion

    }

    public class BombermanViewModelDesignData : BombermanViewModel
    {
        public BombermanViewModelDesignData()
        {
            LoginViewModel = new LoginViewModelDesignData();
            GameRoomViewModel = new GameRoomViewModelDesignData();
            StartedGameViewModel = new StartedGameViewModelDesignData();

            LoginViewModel.IsVisible = true;
            GameRoomViewModel.IsVisible = false;
            StartedGameViewModel.IsVisible = false;
        }
    }
}
