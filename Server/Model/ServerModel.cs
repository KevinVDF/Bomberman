using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.DataContract;
using Common.Interfaces;

namespace Server.Model
{
    public class ServerModel
    {
        public ServerStatus ServerStatus { get; set; }

        public List<PlayerModel> PlayersOnline { get; set; }

        public Game GameCreated { get; set; }

        public void Initialize()
        {
            PlayersOnline = new List<PlayerModel>();
            ServerStatus = ServerStatus.Started;
        }

        public void ConnectUser(IBombermanCallbackService callback, string username)
        {
            //create new Player
            PlayerModel newPlayer = new PlayerModel
            {
                Player = new Player
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Username = username,
                    //Check if its the first user to be connected
                    IsCreator = PlayersOnline.Count == 0
                },
                CallbackService = callback
            };
            //register user to the server
            PlayersOnline.Add(newPlayer);
            //create a list of login to send to client
            List<string> playersNamesList = PlayersOnline.Select(x => x.Player.Username).ToList();
            //Send a success connection to the new user connected with all players online
            newPlayer.CallbackService.OnConnection(newPlayer.Player, playersNamesList);
            //Warning players that a new player is connected and send them the list of all players online
            foreach (PlayerModel player in PlayersOnline.Where(player => player != newPlayer))
            {
                player.CallbackService.OnUserConnected(playersNamesList);
            }
 

        }

        public void StartGame(IBombermanCallbackService callback, string mapPath)
        {
            //create the list of players to pass to client
            List<Player> players = PlayersOnline.Select(playerModel => playerModel.Player).ToList();

            Game newGame = new Game
            {
                Map = new Map
                {
                    MapName = "Custom Map",
                    GridPositions = GenerateGrid(players, mapPath)
                },
                CurrentStatus = GameStatus.Started,
            };
            Server.GameCreated = newGame;
            //send the game to all players (only once)
            foreach (PlayerModel currentPlayer in Server.PlayersOnline)
            {
                currentPlayer.CallbackService.OnGameStarted(newGame);
            }
        }

        public void MoveObjectToLocation(IBombermanCallbackService callback, ActionType actionType)
        {
            PlayerModel player = Server.PlayersOnline.FirstOrDefault(x => x.CallbackService == callback);
            if (player != null)
            {
                switch (actionType)
                {
                    case ActionType.MoveUp:
                        Move(player.Player, 0, -1);
                        break;
                    case ActionType.MoveDown:
                        Move(player.Player, 0, +1);
                        break;
                    case ActionType.MoveRight:
                        Move(player.Player, +1, 0);
                        break;
                    case ActionType.MoveLeft:
                        Move(player.Player, -1, 0);
                        break;
                }
            }
        }

        private List<LivingObject> GenerateGrid(List<Player> players, string mapPath)
        {
            List<LivingObject> matrice = new List<LivingObject>();

            using (StreamReader reader = new StreamReader(mapPath, Encoding.UTF8))
            {
                string objectsToAdd = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");

                for (int y = 0; y < MapSize; y++)
                {
                    for (int x = 0; x < MapSize; x++)
                    {
                        LivingObject livingObject = null;
                        char cell = objectsToAdd[(y * MapSize) + x]; // SinaC: factoring is the key :)   y and x were inverted
                        switch (cell)
                        {
                            case 'u':
                                livingObject = new Wall
                                {
                                    WallType = WallType.Undestructible,
                                    ObjectPosition = new Position
                                    {
                                        PositionX = x,
                                        PositionY = y
                                    }
                                };
                                break;
                            case 'd':
                                livingObject = new Wall
                                {
                                    WallType = WallType.Destructible,
                                    ObjectPosition = new Position
                                    {
                                        PositionX = x,
                                        PositionY = y
                                    }
                                };
                                break;
                            //case 'b' :
                            //    currentlivingObject = new Bonus
                            //    {

                            //    };
                            //    break;
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                                if (players.Count > (int)Char.GetNumericValue(cell))
                                {
                                    livingObject = players[(int)Char.GetNumericValue(cell)];
                                    livingObject.ObjectPosition = new Position
                                    {
                                        PositionX = x,
                                        PositionY = y
                                    };

                                }
                                break;
                        }
                        if (livingObject != null)
                            matrice.Add(livingObject);
                    }
                }
            }
            return matrice;
        }

        public void PlayerAction(IBombermanCallbackService callback, ActionType actionType)
        {
            throw new NotImplementedException();
        }
    }

    public enum ServerStatus
    {
        Started,
        Stopped
    }
}
