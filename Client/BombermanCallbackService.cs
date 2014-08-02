﻿using System;
using System.Collections.Generic;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    //[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanCallbackService : IBombermanCallbackService
    {
        private static readonly ClientProcessor ClientProcessor = new ClientProcessor();

        public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            ClientProcessor.OnUserConnected(player, loginsList, canStartGame);
        }

        public void OnGameStarted(Game newGame)
        {
            ClientProcessor.OnGameStarted(newGame);
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            ClientProcessor.OnMove(objectToMoveBefore, objectToMoveAfter);
        }
    }
}
