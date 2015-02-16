using System;
using System.Collections.Generic;
using Client.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace Client
{
    //[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class BombermanCallbackService : IBombermanCallbackService
    {
        private static readonly ClientModel ClientModel = new ClientModel();

        public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            ClientModel.OnUserConnected(player, loginsList, canStartGame);
        }

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

        public void OnBombDropped(Position bombPosition)
        {
            ClientModel.OnBombDropped(newBomb);
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            ClientModel.OnMove(objectToMoveBefore, objectToMoveAfter);
        }
    }
}
