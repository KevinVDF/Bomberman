using Common.DataContract;
using Common.Log;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class GameManager : IGameManager
    {
        public Game CreateNewGame(Map map)
        {
            if (map == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Create game failed due to unknown map");
                return null;
            }
            //create the game to return
            Game newGame = new Game
            {
                CurrentStatus = GameStatus.Created,
                Map = map
            };

            Log.WriteLine(Log.LogLevels.Info, "Game created");

            return newGame;
        }
    }
}
