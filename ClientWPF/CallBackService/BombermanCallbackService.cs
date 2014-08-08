﻿using System;
using System.Collections.Generic;
using ClientWPF.Logic;
using ClientWPF.ViewModels;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.CallBackService
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        public ClientProcessor ClientProcessor = new ClientProcessor();

        public void OnConnection(Player mePlayer, List<string> logins)
        {
            ClientProcessor.OnConnection(mePlayer, logins);
        }

        public void OnUserConnected(List<string> logins)
        {
            ClientProcessor.OnUserConnected(logins);
        }

        public void OnGameStarted(Game newGame)
        {
            //ClientProcessor.OnGameStarted(newGame); todo
        }

        public void OnPlayerMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            //ClientProcessor.OnPlayerMove(objectToMoveBefore, objectToMoveAfter);todo
        }

        public void OnBombDropped(Position bombPosition)
        {
            //ClientProcessor.OnBombDropped(bombPosition todo
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            //ClientProcessor.OnMove(objectToMoveBefore, objectToMoveAfter);todo
        }
    }
}
