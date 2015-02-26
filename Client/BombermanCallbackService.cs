using System;
using System.Collections.Generic;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    public class BombermanCallbackService : IBombermanCallbackService
    {

        private readonly ClientProcessor _clientProcessor;

        public BombermanCallbackService(ClientProcessor processor)
        {
            _clientProcessor = processor;
        }

        public void OnError(string messageError, ErrorType errorType)
        {
            _clientProcessor.OnError(messageError, errorType);
        }

        public void OnConnection(Guid id, IEnumerable<string> logins)
        {
            _clientProcessor.OnConnection(id, logins);
        }

        public void OnUserConnected(IEnumerable<string> logins)
        {
            _clientProcessor.OnUserConnected(logins);
        }

        public void OnUserDisconnected(IEnumerable<string> logins)
        {
            _clientProcessor.OnUserDisconnected(logins);
        }

        public void OnGameStarted(Player player, Game newGame)
        {
            _clientProcessor.OnGameStarted(player, newGame);
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
