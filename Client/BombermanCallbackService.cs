using System;
using System.Collections.Generic;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        public void OnErrorConnection(string messageError)
        {
            throw new NotImplementedException();
        }

        public void OnConnection(Player mePlayer, IEnumerable<string> logins)
        {
            throw new NotImplementedException();
        }

        public void OnUserConnected(IEnumerable<string> logins)
        {
            throw new NotImplementedException();
        }

        public void OnGameStarted(Game newGame)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            throw new NotImplementedException();
        }

        public void OnBombDropped(Bomb newBomb)
        {
            throw new NotImplementedException();
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerDeath(Player player)
        {
            throw new NotImplementedException();
        }

        public void OnMyDeath()
        {
            throw new NotImplementedException();
        }

        public void OnDraw()
        {
            throw new NotImplementedException();
        }

        public void OnWin()
        {
            throw new NotImplementedException();
        }

        public void OnCanRestartGame()
        {
            throw new NotImplementedException();
        }
    }
}
