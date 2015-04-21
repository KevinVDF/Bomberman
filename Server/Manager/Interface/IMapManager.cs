
using Common.DataContract;

namespace Server.Manager.Interface
{
    public interface IMapManager
    {
        Map GenerateMapFromTXTFile(string mapName);

        Position MovePlayer(User user, int stepX, int stepY);

        void PlayerDropBomb(Player player, Bomb bomb);
    }
}
