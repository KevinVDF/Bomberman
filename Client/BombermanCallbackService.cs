using System;
using System.Collections.Generic;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    public class BombermanCallbackService : IBombermanCallbackService
    {

        private readonly ClientProcessor ClientProcessor;

        public BombermanCallbackService()
        {
            ClientProcessor = new ClientProcessor();
        }

        public void OnError(string messageError, ErrorType errorType)
        {
            ClientProcessor.OnError(messageError, errorType);
        }

        public void OnConnection(Player mePlayer, IEnumerable<string> logins)
        {
            ClientProcessor.OnConnection(mePlayer, logins);
        }

        public void OnUserConnected(IEnumerable<string> logins)
        {
            ClientProcessor.OnUserConnected(logins);
        }

        public void OnUserDisconnected(IEnumerable<string> logins)
        {
            ClientProcessor.OnUserDisconnected(logins);
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
