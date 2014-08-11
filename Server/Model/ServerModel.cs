using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace Server.Model
{
    public class ServerModel
    {

        #region properties

        public int MapSize = 10;

        public ServerStatus ServerStatus;

        public List<PlayerModel> PlayersOnline;

        public Game GameCreated { get; set; }

        public List<PlayerModel> PlayersDisconnected;

        public string MapPath;

        public int idCount = 1;

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
                    Id = idCount++,
                    Username = username,
                    //Check if its the first user to be connected
                    IsCreator = PlayersOnline.Count == 0,
                    BombPower = 1,
                    MaxBombCount = 1
                },
                CallbackService = callback,
                Alife = true
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
            if(mapPath != "")
                MapPath = mapPath;

            InitializeDisconnected();

            List<Player> players = PlayersOnline.Select(playerModel => playerModel.Player).ToList();

            Game newGame = new Game
            {
                Map = new Map
                {
                    MapName = "Custom Map",
                    GridPositions = GenerateGrid(players)
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
                    playerModel.Alife = true;
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
            Log.WriteLine(Log.LogLevels.Info, "The game is started");
        }

        private List<LivingObject> GenerateGrid(List<Player> players)
        {
            List<LivingObject> matrice = new List<LivingObject>();

            using (StreamReader reader = new StreamReader(MapPath, Encoding.UTF8))
            {
                string objectsToAdd = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");

                for (int y = 0; y < MapSize; y++)
                {
                    for (int x = 0; x < MapSize; x++)
                    {
                        LivingObject livingObject = null;
                        char cell = objectsToAdd[(y*MapSize) + x]; // SinaC: factoring is the key :)   y and x were inverted
                        switch (cell)
                        {
                            case 'u':
                                livingObject = new Wall
                                {
                                    WallType = WallType.Undestructible,
                                    Position = new Position
                                    {
                                        X = x,
                                        Y = y
                                    }
                                    , Id = idCount++
                                };
                                Log.WriteLine(Log.LogLevels.Warning, "New Wall undestructible created");
                                break;
                            case 'd':
                                livingObject = new Wall
                                {
                                    WallType = WallType.Destructible,
                                    Position = new Position
                                    {
                                        X = x,
                                        Y = y
                                    }
                                    , Id = idCount++
                                };
                                Log.WriteLine(Log.LogLevels.Warning, "New Wall destructible created");
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
                                if (players.Count > (int) Char.GetNumericValue(cell))
                                {
                                    livingObject = players[(int) Char.GetNumericValue(cell)];
                                    livingObject.Position = new Position
                                    {
                                        X = x,
                                        Y = y,
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

            // Get object at future player location
            LivingObject collider = GameCreated.Map.GridPositions.FirstOrDefault(x => player.Position.Y + stepY == x.Position.Y
                                                                                      && player.Position.X + stepX == x.Position.X
                );
            // Can't go thru wall or bomb
            if (collider is Wall || collider is Bomb)
                return;

            GameCreated.Map.GridPositions.Remove(player);

            Position newPosition = new Position
            {
                X = player.Position.X + stepX,
                Y = player.Position.Y + stepY
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

            player.Position.Y += stepY;
            player.Position.X += stepX;
            GameCreated.Map.GridPositions.Add(player);
        }

        private void DropBomb(Player player)
        {
            int count = GameCreated.Map.GridPositions.Count(x => x is Bomb && ((Bomb) x).PlayerId == player.Id);
            if (count >= player.MaxBombCount)
            {
                Log.WriteLine(Log.LogLevels.Error, "player tries to drop a bomb but number of bomb to high");
                return;
            }
                
            Bomb newBomb = new Bomb
            {
                Id = idCount++,
                PlayerId = player.Id,
                Power = player.BombPower,
                Position = new Position
                {
                    X = player.Position.X,
                    Y = player.Position.Y
                }
            };
            GameCreated.Map.GridPositions.Add(newBomb);

            foreach (PlayerModel playerModel in PlayersOnline.Where(x=> x.Alife))
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
            //make the bomb explode
            Timer t = new Timer(BombExplode, newBomb, 1500, Timeout.Infinite);
            CheckForRestart();

        }

        private void CheckForRestart()
        {
            if (PlayersOnline.Count(x => x.Alife) == 0)
                PlayersOnline.First(x => x.Player.IsCreator).CallbackService.OnCanRestartGame();
        }

        public void RestartGame()
        {
            StartGame("");
        }

        private void BombExplode(object bomb)
        {
            Bomb bombToExplode = bomb as Bomb;

            if (bombToExplode == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Bomb is null => WTF ??" );
                return;
            }

            List<LivingObject> impacted = new List<LivingObject>();
            List<LivingObject> tempList = new List<LivingObject>();
            //research impact 
            for (int direction = 0; direction < 4; direction++)
            {
                for (int i = 1; i <= bombToExplode.Power; i++)
                {
                    tempList = new List<LivingObject>();
                    switch (direction)
                    {
                            //up
                        case 0:
                            tempList.AddRange(GameCreated.Map.GridPositions.Where(x => x.Position.Y == bombToExplode.Position.Y - i
                                                                                       && x.Position.X == bombToExplode.Position.X).ToList());
                            break;
                            //down
                        case 1:
                            tempList.AddRange(GameCreated.Map.GridPositions.Where(x => x.Position.Y == bombToExplode.Position.Y + i
                                                                                       && x.Position.X == bombToExplode.Position.X).ToList());
                            break;
                            //left
                        case 2:
                            tempList.AddRange(GameCreated.Map.GridPositions.Where(x => x.Position.X == bombToExplode.Position.X - i
                                                                                       && x.Position.Y == bombToExplode.Position.Y).ToList());
                            break;
                            //right
                        default:
                            tempList.AddRange(GameCreated.Map.GridPositions.Where(x => x.Position.X == bombToExplode.Position.X + i
                                                                                       && x.Position.Y == bombToExplode.Position.Y).ToList());
                            break;
                    }
                    //if we encountered an empty space
                    if (tempList.Count == 0) 
                        continue;
                    //if we encountered a wall undestructible don't need to go further
                    if (!IsImpacted(tempList))
                        break;
                    impacted.AddRange(tempList);
                }
            }
            //check if the players'bomb doesn't move
            tempList = new List<LivingObject>();
            tempList.AddRange(GameCreated.Map.GridPositions.Where(x => x.Position.X == bombToExplode.Position.X
                                                                                       && x.Position.Y == bombToExplode.Position.Y).ToList());
            impacted.AddRange(tempList);

            GameCreated.Map.GridPositions.Remove(bombToExplode);
            GameCreated.Map.GridPositions.RemoveAll(impacted.Contains);

            //warn all players still alive
            foreach (PlayerModel playerModel in PlayersOnline)
            {
                try
                {
                    //warn all players that a bomb exploded
                    playerModel.CallbackService.OnBombExploded(bombToExplode, impacted);

                    //if the bomb touch all players left 
                    if (impacted.Count(x => x is Player) == GameCreated.Map.GridPositions.Count(x => x is Player) && playerModel.Alife)
                    {
                        playerModel.CallbackService.OnDraw();
                        playerModel.Alife = false;
                    }
                    else
                    {
                        //if its the last player standing then lets warn him he won
                        if (impacted.Count(x => x is Player) == 1
                            && impacted.Count(x => x is Player && ((Player) x).CompareId(playerModel.Player)) > 0
                            && GameCreated.Map.GridPositions.Count(x => x is Player) == 1)
                        {
                            playerModel.CallbackService.OnWin();
                        }
                        else
                        {
                            //if the bomb touch the current player
                            if (impacted.Count(x => x is Player && ((Player) x).CompareId(playerModel.Player)) > 0)
                            {
                                playerModel.CallbackService.OnMyDeath();
                                playerModel.Alife = false;
                            }
                            //if someone else is dead
                            if (impacted.Count(x => x is Player && !((Player) x).CompareId(playerModel.Player)) > 0)
                            {
                                playerModel.CallbackService.OnPlayerDeath(playerModel.Player);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection error with player " + playerModel.Player.Username);
                    Log.WriteLine(Log.LogLevels.Error, "ConnectUser callback error :" + ex.Message);
                    PlayersDisconnected.Add(playerModel);
                }
            }
           
            foreach (PlayerModel playerDisconnected in PlayersDisconnected)
            {
                PlayersOnline.Remove(playerDisconnected);
            }
        }

        private static bool IsImpacted(List<LivingObject> list)
        {
            return list.All(livingObject => !(livingObject is Wall) || ((Wall) livingObject).WallType != WallType.Undestructible);//TODO
        }

        #endregion Methods

        
    }

    public enum ServerStatus
    {
        Started,
        Stopped
    }
}
