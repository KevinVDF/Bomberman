using System.Collections.Generic;
using System.ServiceModel;
using Common.DataContract;


namespace Common.Interfaces
{
    
    public interface IBombermanCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void OnConnection(Player mePlayer, List<string> logins);
        [OperationContract(IsOneWay = true)]
        void OnUserConnected(List<string> logins);
        [OperationContract(IsOneWay = true)]
        void OnGameStarted(Game newGame);
        [OperationContract(IsOneWay = true)]
        void OnPlayerMove(Player player, Position newPosition, ActionType actionType);
        [OperationContract(IsOneWay = true)]
        void OnBombDropped(Bomb newBomb);
        [OperationContract]
        void OnBombExploded(Bomb bomb, List<LivingObject> impacted);
        [OperationContract]
        void OnMyDeath();
        [OperationContract]
        void OnPlayerDeath(Player player);
        [OperationContract]
        void OnDraw();
        [OperationContract]
        void OnWin();
        [OperationContract]
        void OnCanRestartGame();
    }
}
