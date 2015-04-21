using System;
using System.Configuration;
using Common.DataContract;
using Common.Log;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class GameManager : IGameManager
    {
        private Game _game;
        private IMapManager _mapManager;

        public GameManager(IMapManager mapManager)
        {
            _mapManager = mapManager;
        }

        public void CreateNewGame()
        {
            string mapName = ConfigurationManager.AppSettings["mapName"];

            //If map is null or empty
            if (string.IsNullOrEmpty(mapName))
            {
                string errorMessage = "Unknown map name";
                Log.WriteLine(Log.LogLevels.Error, errorMessage);
                return;
            }

            //create the game to return
            _game = new Game
            {
                CurrentStatus = GameStatus.Created,
                Map = _mapManager.GenerateMapFromTXTFile(mapName)
            };

            Log.WriteLine(Log.LogLevels.Info, "Game created");
        }

        public Game GetCurrentGame()
        {
            return _game;
        }
    }
}
