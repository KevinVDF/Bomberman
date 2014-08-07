using System;
using System.Collections.Generic;
using ClientWPF.ViewModels;
using Common.DataContract;

namespace ClientWPF.Logic
{
    public class ClientProcessor
    {
        public Player Player { get; set; }

        public Map Map { get; set; }

        public static BombermanViewModel BombermanViewModel { get; set; }

        public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            BombermanViewModel.OnUserConnected(player, loginsList, canStartGame);
        }

        public void OnGameStarted(Game newGame)
        {

        }
    }
}
