using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace Server.Model
{
    public class ServerModel
    {

        #region properties

        public int MapSize = 10;

        public ServerStatus ServerStatus { get; set; }

        public List<PlayerModel> PlayersOnline { get; set; }

        public Game GameCreated { get; set; }

        public List<PlayerModel> PlayersDisconnected { get; set; }

        #endregion properties

        public ServerModel()
        {
            PlayersOnline = new List<PlayerModel>();
            PlayersDisconnected = new List<PlayerModel>();
            ServerStatus = ServerStatus.Started;
        }

        #region Methods

        private void InitializeDisconnected()
        {
            PlayersDisconnected = new List<PlayerModel>();
        }

        public void ConnectUser(IBombermanCallbackService callback, string username)
        {
            InitializeDisconnected();

            //create new Player
            PlayerModel newPlayer = new PlayerModel
            {
                Player = new Player
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    Username = username,
                    //Check if its the first user to be connected
                    IsCreator = PlayersOnline.Count == 0,
                    BombPower = 1,
                    BombNumber = 1
                },
                CallbackService = callback
            };
            Log.WriteLine(Log.LogLevels.Info, "New player connected : " + username);

            //register user to the server
            PlayersOnline.Add(newPlayer);
            //create a list of login to send to client
            List<string> playersNamesList = PlayersOnline.Select(x => x.Player.Username).ToList();
            //Send a success connection to the new user connected with all players online
            newPlayer.CallbackService.OnConnection(newPlayer.Player, playersNamesList);
            //Warning players that a new player is connected by sending them the list of all players online
            foreach (PlayerModel player in PlayersOnline.Where(player => player != newPlayer))
            {
                try
                {
                    player.CallbackService.OnUserConnected(playersNamesList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + player.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    PlayersDisconnected.Add(player);
                }
            }
            //withdraw players with problems from players online
            foreach (PlayerModel playerDisconnected in PlayersDisconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
            }
        }

        public void StartGame(string mapPath)
        {
            InitializeDisconnected();

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
            GameCreated = newGame;
            //send the game to all players (only once)
            foreach (PlayerModel playerModel in PlayersOnline)
            {
                try
                {
                    playerModel.CallbackService.OnGameStarted(newGame);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + playerModel.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    PlayersDisconnected.Add(playerModel);
                }
            }
            //withdraw players with problems from players online
            foreach (PlayerModel playerDisconnected in PlayersDisconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
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
            PlayerModel player = PlayersOnline.FirstOrDefault(x => x.CallbackService == callback);

            if (player != null)
            {
                switch (actionType)
                {
                    case ActionType.MoveUp:
                        MovePlayer(player.Player, 0, -1, actionType);
                        break;
                    case ActionType.MoveDown:
                        MovePlayer(player.Player, 0, +1, actionType);
                        break;
                    case ActionType.MoveRight:
                        MovePlayer(player.Player, +1, 0, actionType);
                        break;
                    case ActionType.MoveLeft:
                        MovePlayer(player.Player, -1, 0, actionType);
                        break;
                    case ActionType.DropBomb:
                        DropBomb(player.Player);
                        break;
                }
            }
        }

        private void MovePlayer(Player player, int stepX, int stepY, ActionType actionType)
        {
            InitializeDisconnected();

            // Get object at future player location TODO
            LivingObject collider = GameCreated.Map.GridPositions.FirstOrDefault(x => player.ObjectPosition.PositionY + stepY == x.ObjectPosition.PositionY
                                                                                             && player.ObjectPosition.PositionX + stepX == x.ObjectPosition.PositionX
                                                                                             );
            // Can't go thru wall ir player
            if (collider is Wall)
                return;
            
            GameCreated.Map.GridPositions.Remove(player);

            Position newPosition = new Position
            {
                PositionX = player.ObjectPosition.PositionX + stepX,
                PositionY = player.ObjectPosition.PositionY + stepY
            };

            // Send new player position to players
            foreach (PlayerModel playerModel in PlayersOnline)
            {
                try
                {
                    playerModel.CallbackService.OnPlayerMove(player, newPosition, actionType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + playerModel.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    PlayersDisconnected.Add(playerModel);
                }
            }
            //withdraw players with problems from players online
            foreach (PlayerModel playerDisconnected in PlayersDisconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
            }

            player.ObjectPosition.PositionY += stepY;
            player.ObjectPosition.PositionX += stepX;
            GameCreated.Map.GridPositions.Add(player);
        }

        private void DropBomb(Player player)
        {
            Bomb newBomb = new Bomb
            {
                PlayerId = player.Id,
                Power = player.BombPower,
                ObjectPosition = player.ObjectPosition
            };
            GameCreated.Map.GridPositions.Add(newBomb);

            foreach (PlayerModel playerModel in PlayersOnline)
            {
                try
                {
                    playerModel.CallbackService.OnBombDropped(newBomb);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + playerModel.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    PlayersDisconnected.Add(playerModel);
                }
            }
            //withdraw players with problems from players online
            foreach (PlayerModel playerDisconnected in PlayersDisconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
            }
        }

        #endregion Methods
    }

    public enum ServerStatus
    {
        Started,
        Stopped
    }
}
