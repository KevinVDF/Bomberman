
using Common.DataContract;

namespace Server.Manager.Interface
{
    public interface IGameManager
    {
        void CreateNewGame();

        Game GetCurrentGame();
    }
}
