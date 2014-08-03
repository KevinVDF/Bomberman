using System;
using System.Collections.Generic;
using ClientWPF.Logic;
using ClientWPF.ViewModels;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.CallBackService
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        private static readonly ClientProcessor ClientProcessor = new ClientProcessor();

        public static BombermanViewModel BombermanViewModel;

        public BombermanCallbackService(BombermanViewModel bombermanViewmodel )
        {
            BombermanViewModel = bombermanViewmodel;
        }

        public void OnUserConnected(Player newPlayer, List<String> loginsList, bool canStartGame)
        {
            BombermanViewModel.OnUserConnected(newPlayer, loginsList, canStartGame);
        }

        public void OnGameStarted(Game newGame)
        {
            BombermanViewModel.OnGameStarted(newGame);
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            ClientProcessor.OnMove(objectToMoveBefore, objectToMoveAfter);
        }
    }
}
