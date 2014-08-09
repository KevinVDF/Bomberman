﻿using System;
using System.Collections.Generic;
using System.Threading;
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

        public IBombermanService Proxy { get; set; }

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
            var factory = new DuplexChannelFactory<IBombermanService>(context, "netTcpBinding_IBombermanService");
            Proxy = factory.CreateChannel();
            LoginViewModel.Initialize(true, Proxy);
            GameRoomViewModel.Initialize(false, Proxy);
            StartedGameViewModel.Initialize(false, Proxy);
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
            //GameRoomViewModel.IsVisible = false;
            //StartedGameViewModel.IsVisible = true;
            ////set the new game retreive from server
            //StartedGameViewModel.RegisterGame(newGame);
            //StartedGameViewModel.InitializePlayer(Player); todo
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            ////if before is player and is "me" then update global player
            //if (objectToMoveBefore is Player && Player.CompareId(objectToMoveBefore))
            //    Player = objectToMoveAfter as Player;
            ////Map.GridPositions.Remove(objectToMoveBefore);
            //PlayerItem objectToMove = StartedGameViewModel.MapViewModel.LivingObjects.FirstOrDefault(player => player.PositionX == (objectToMoveBefore.ObjectPosition.PositionX*50)+100 && player.PositionY == objectToMoveBefore.ObjectPosition.PositionY*50) as PlayerItem;
            //if (objectToMove != null)
            //{
            //    Timer timer; 
            //    for(int i=3; i>0; i++)
            //    {

            //        timer = new Timer(MovePlayer(objectToMove, objectToMoveAfter, i));
                   
                     
            //    }
            //}todo
        }

        public void MoveObjectToLocation(ActionType actionType)
        {
            //Proxy.MoveObjectToLocation(Player.Id, actionType); todo
        }

        private TimerCallback MovePlayer(PlayerItem objectToMove, LivingObject objectToMoveAfter, int i)
        {
             //objectToMove.PositionX = objectToMoveAfter.ObjectPosition.PositionX/i;
             //       objectToMove.PositionY = objectToMoveAfter.ObjectPosition.PositionY/i;
             //       objectToMove.ImageInUse = objectToMove.Down.Images[i - 1];todo
            return null;
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
