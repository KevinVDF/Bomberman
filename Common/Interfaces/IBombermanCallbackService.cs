﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.DataContract;


namespace Common.Interfaces
{
    
    public interface IBombermanCallbackService
    {
        //send error message on fail connection
        [OperationContract(IsOneWay = true)]
        void OnError(string messageError, ErrorType errorType);
        //when i connect myself
        [OperationContract(IsOneWay = true)]
        void OnConnection(Guid ID, IEnumerable<string> logins);
        //when an other player connects
        [OperationContract(IsOneWay = true)]
        void OnUserConnected(IEnumerable<string> logins);
        //when an other player disconnects
        [OperationContract(IsOneWay = true)]
        void OnUserDisconnected(IEnumerable<string> logins);
        //when the creator start the game
        [OperationContract(IsOneWay = true)]
        void OnGameStarted(Player player, Game newGame);
        //when any player makes a move
        [OperationContract(IsOneWay = true)]
        void OnPlayerMove(Player player, Position newPosition, ActionType actionType);
        //when an user drop a bomb
        [OperationContract(IsOneWay = true)]
        void OnBombDropped(Bomb newBomb);
        //when a bomb explode
        [OperationContract]
        void OnBombExploded(Bomb bomb, List<LivingObject> impacted);
        [OperationContract]
        void OnPlayerDeath(Player player);
        [OperationContract]
        void OnMyDeath();
        [OperationContract]
        void OnDraw();
        [OperationContract]
        void OnWin();
        [OperationContract]
        void OnCanRestartGame();
    }
}
