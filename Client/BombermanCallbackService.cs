using System;
using System.Collections.Generic;
using Client.Logic;
using Client.Model;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        private static readonly ClientModel ClientModel = new ClientModel();

        public void OnConnection(Player mePlayer, List<string> logins)
        {
            ClientModel.OnConnection(mePlayer, logins);
        }

        public void OnUserConnected(List<string> logins)
        {
            ClientModel.OnUserConnected(logins);
        }

        public void OnGameStarted(Game newGame)
        {
            ClientModel.OnGameStarted(newGame);
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            ClientModel.OnPlayerMove(player, newPosition, actionType);
        }

        public void OnBombDropped(Bomb newBomb)
        {
            ClientModel.OnBombDropped(newBomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            ClientModel.OnBombExploded(bomb, impacted);
        }

        public void OnPlayerDeath(Player player)
        {
            ClientModel.OnPlayerDeath(player);
        }

        public void OnMyDeath()
        {
            ClientModel.OnMyDeath();
        }

        public void OnDraw()
        {
            ClientModel.OnDraw();
        }

        public void OnWin()
        {
            ClientModel.OnWin();
        }

        public void OnCanRestartGame()
        {
            ClientModel.OnCanRestartGame();
        }

        //public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        //{
        //    ClientProcessor.OnUserConnected(player, loginsList, canStartGame);
        //}

        //public void OnConnection(Player mePlayer, List<string> logins)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnUserConnected(List<string> logins)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnGameStarted(Game newGame)
        //{
        //    ClientProcessor.OnGameStarted(newGame);
        //}

        //public void OnPlayerMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnBombDropped(Position bombPosition)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        //{
        //    ClientProcessor.OnMove(objectToMoveBefore, objectToMoveAfter);
        //}
    }
}
