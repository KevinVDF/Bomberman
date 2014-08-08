﻿using System.Collections.Generic;
using System.ServiceModel;
using Common.DataContract;


namespace Common.Interfaces
{
    
    public interface IBombermanCallbackService
    {
        //when i connect myself
        [OperationContract(IsOneWay = true)]
        void OnConnection(Player mePlayer, List<string> logins);
        //when an other player connects
        [OperationContract(IsOneWay = true)]
        void OnUserConnected(List<string> logins);
        //when the creator start the game
        [OperationContract(IsOneWay = true)]
        void OnGameStarted(Game newGame);
        //when any player makes a move
        [OperationContract(IsOneWay = true)]
        void OnPlayerMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter);
        //when an user drop a bomb
        [OperationContract(IsOneWay = true)]
        void OnBombDropped(Position bombPosition);
    }
}