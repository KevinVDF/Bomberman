using System.Collections.Generic;
using ClientWPF.Logic;
using ClientWPF.ViewModels;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.CallBackService
{
    public class BombermanCallbackService : IBombermanCallbackService
    {
        public ClientModel ClientModel;

        public BombermanCallbackService(BombermanViewModel bombermanViewModel)
        {

            ClientModel = new ClientModel(bombermanViewModel);
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

        public void OnBombDropped(Bomb newBomb)
        {
            ClientModel.OnBombDropped(newBomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            ClientModel.OnBombExploded(bomb, impacted);
        }
    }
}
