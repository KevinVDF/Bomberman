using Common.DataContract;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class GameManager : IGameManager
    {
        public static Game CreateNewGame()
        {
            //create the game to return
            Game newGame = new Game
            {
                CurrentStatus = GameStatus.Created,
            };

            return newGame;
        }

        public static void AddPlayerToGame(Player playerToAdd, Game game)
        {
            
        }

        public static GameStatus GetGameStatus(Game game)
        {
            return game.CurrentStatus;
        }
    }
}
