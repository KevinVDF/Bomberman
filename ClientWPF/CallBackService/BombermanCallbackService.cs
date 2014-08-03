using System;
using System.Collections.Generic;
using ClientWPF.Logic;
<<<<<<< HEAD
using ClientWPF.ViewModels;
=======
<<<<<<< HEAD
using ClientWPF.ViewModels;
=======
>>>>>>> origin/master
>>>>>>> origin/master
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.CallBackService
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        private static readonly ClientProcessor ClientProcessor = new ClientProcessor();

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master
        public static BombermanViewModel BombermanViewModel;

        public BombermanCallbackService(BombermanViewModel bombermanViewmodel )
        {
            BombermanViewModel = bombermanViewmodel;
        }

        public void OnUserConnected(Player newPlayer, List<String> loginsList, bool canStartGame)
        {
            BombermanViewModel.OnUserConnected(newPlayer, loginsList, canStartGame);
<<<<<<< HEAD
=======
=======
        public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            ClientProcessor.OnUserConnected(player, loginsList, canStartGame);
>>>>>>> origin/master
>>>>>>> origin/master
        }

        public void OnGameStarted(Game newGame)
        {
<<<<<<< HEAD
            BombermanViewModel.OnGameStarted(newGame);
=======
            ClientProcessor.OnGameStarted(newGame);
>>>>>>> origin/master
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            ClientProcessor.OnMove(objectToMoveBefore, objectToMoveAfter);
        }
    }
}
