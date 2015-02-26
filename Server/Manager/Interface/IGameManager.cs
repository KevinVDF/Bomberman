
using Common.DataContract;

namespace Server.Manager.Interface
{
    public interface IGameManager
    {
        Game CreateNewGame(Map map);
    }
}
