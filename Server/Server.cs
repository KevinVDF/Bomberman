using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;
using Server.Manager;
using Server.Manager.Interface;

namespace Server
{
    public class Server
    {
        public ServerStatus ServerStatus { get; private set; }
        public IUserManager UserManager { get; private set; }
        public ICallbackManager CallbackManager { get; private set; }
        public IGameManager GameManager { get; private set; }
        public IMapManager MapManager { get; private set; }

        public Server(IUserManager userManager, ICallbackManager callbackManager, IGameManager gameManager, IMapManager mapManager)
        {
            ServerStatus = ServerStatus.Started;
            UserManager = userManager;
            CallbackManager = callbackManager;
            GameManager = gameManager;
            MapManager = mapManager;
        }
        //OKAY
        public void ConnectUser(IBombermanCallbackService callback, string username)
        {
            //if callback is null => big problem
            if (callback == null)
            {
                string errorMessage = string.Format("Problem with callback for the player {0}", username);
                Log.WriteLine(Log.LogLevels.Error, errorMessage);
                return;
            }

            //if username already taken then refuse.
            if (UserManager.IsUsernameAlreadyUsed(username))
            {
                string errorMessage = string.Format("Username {0} is already Taken.", username);
                Log.WriteLine(Log.LogLevels.Error,errorMessage);
                CallbackManager.SendError(callback, errorMessage, ErrorType.Connection);
                return;
            }

            //create new User
            User newUser = UserManager.CreateNewUser(username, callback);

            if (newUser == null)
            {
                string errorMessage = string.Format("Username {0} is empty or already Taken.", username);
                Log.WriteLine(Log.LogLevels.Info, errorMessage);
                CallbackManager.SendError(callback, errorMessage, ErrorType.Connection);
                return;    
            }

            Log.WriteLine(Log.LogLevels.Info, "New user created : {0}", username);

            //Send the list of username to the new player created
            CallbackManager.SendUsernameListToNewPlayer(newUser, UserManager.GetListOfUsername());
            //Warning players that a new player is connected by sending them the list of all players online
            CallbackManager.SendUsernameListToAllOtherUserAfterConnection(UserManager.GetAllOtherUsers(newUser), UserManager.GetListOfUsername());
        }
        //OKAY
        public void DisconnectUser(IBombermanCallbackService callback, string username)
        {
            //if callback is null => big problem
            if (callback == null)
            {
                string errorMessage = string.Format("Problem with callback for the player {0}", username);
                Log.WriteLine(Log.LogLevels.Error, errorMessage);
                return;
            }
            //if username is empty
            if (string.IsNullOrEmpty(username))
            {
                string errorMessage = string.Format("Username is empty.");
                Log.WriteLine(Log.LogLevels.Info, errorMessage);
                CallbackManager.SendError(callback, errorMessage, ErrorType.Connection);
            }

            User userToDelete = UserManager.GetUserByUsername(username);

            if (userToDelete == null)
            {
                string errorMessage = string.Format("Username {0} is unknown.", username);
                Log.WriteLine(Log.LogLevels.Info, errorMessage);
                CallbackManager.SendError(callback, errorMessage, ErrorType.Connection);
                return;    
            }

            UserManager.DeleteUser(userToDelete);

            Log.WriteLine(Log.LogLevels.Info, "User deleted : {0}", username);

            //Warning players that a new player is connected by sending them the list of all players online
            CallbackManager.SendUsernameListToAllOtherUserAfterDisconnection(UserManager.GetAllOtherUsers(userToDelete), UserManager.GetListOfUsername());

        }
        ////OKAY
        public void StartNewGame(string mapName)
        {
            List<Player> players = PlayersOnline.Select(playerModel => playerModel.Player).ToList();

            Map map = GenerateMap(players);

            if (map == null)
                Log.WriteLine(Log.LogLevels.Error, "Error while reading map {0} -> game not started", mapName);
            else
            {
                Game newGame = new Game
                {
                    Map = map,
                    CurrentStatus = GameStatus.Started,
                };
                GameCreated = newGame;
                Log.WriteLine(Log.LogLevels.Debug, "New Game created");
                //send the game to all players (only once)
                ExceptionFreeAction(PlayersOnline, player =>
                {
                    player.CallbackService.OnGameStarted(newGame);
                    player.Alive = true;
                });
            }
        }
        ////OKAY but TODO for bonuses
        //private Map GenerateMap(List<Player> players)
        //{
        //    int mapSize;
        //    Map map = new Map();
        //    List<LivingObject> matrice = new List<LivingObject>();

        //    using (StreamReader reader = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["MapPath"],MapName), Encoding.UTF8))
        //    {
        //        // Read size
        //        string size = reader.ReadLine();
        //        bool isSizeValid = int.TryParse(size, out mapSize);

        //        if (!isSizeValid)
        //        {
        //            Log.WriteLine(Log.LogLevels.Error, "Invalid map size {0}", size);
        //            return null;
        //        }
        //        // Read map
        //        string objectsToAdd = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");

        //        for (int y = 0; y < mapSize; y++)
        //        {
        //            for (int x = 0; x < mapSize; x++)
        //            {
        //                LivingObject livingObject = null;
        //                char cell = objectsToAdd[(y * mapSize) + x];
        //                switch (cell)
        //                {
        //                    case 'u':
        //                        livingObject = new Wall
        //                        {
        //                            WallType = WallType.Undestructible,
        //                            Position = new Position
        //                            {
        //                                X = x,
        //                                Y = y
        //                            }
        //                            ,
        //                            ID = IdCount++
        //                        };
        //                        Log.WriteLine(Log.LogLevels.Debug, "New undestructible wall created");
        //                        break;
        //                    case 'd':
        //                        livingObject = new Wall
        //                        {
        //                            WallType = WallType.Destructible,
        //                            Position = new Position
        //                            {
        //                                X = x,
        //                                Y = y
        //                            }
        //                            ,
        //                            ID = IdCount++
        //                        };
        //                        Log.WriteLine(Log.LogLevels.Debug, "New destructible wall created");
        //                        break;
        //                    //case 'b' :
        //                    //    currentlivingObject = new Bonus
        //                    //    {

        //                    //    };
        //                    //    break;
        //                    case '0':
        //                    case '1':
        //                    case '2':
        //                    case '3':
        //                        if (players.Count > (int)Char.GetNumericValue(cell))
        //                        {
        //                            livingObject = players[(int)Char.GetNumericValue(cell)];
        //                            livingObject.Position = new Position
        //                            {
        //                                X = x,
        //                                Y = y,
        //                            };
        //                        }
        //                        break;
        //                }
        //                if (livingObject != null)
        //                    matrice.Add(livingObject);
        //            }
        //        }
        //    }
        //    map.LivingObjects = matrice;
        //    map.MapName = MapName;
        //    map.MapSize = mapSize;

        //    return map;
        //}
        ////OKAY
        //public void PlayerAction(IBombermanCallbackService callback, ActionType actionType)
        //{
        //    //retreive the player who made the action
        //    User player = PlayersOnline.FirstOrDefault(x => x.CallbackService == callback);

        //    //not supposed to happend but okay
        //    if (GameCreated.CurrentStatus == GameStatus.Stopped)
        //    {
        //        Log.WriteLine(Log.LogLevels.Warning, "Game stopped -> no request from client !!!");
        //        return;
        //    }
        //    //not supposed to happend not okay at all
        //    if (player == null)
        //    {
        //        Log.WriteLine(Log.LogLevels.Error, "Player who made the action is null... shouldn't be !!!");
        //        return; 
        //    }
                
        //    //not supposed to happend but okay
        //    if (!player.Alive)
        //    {
        //        Log.WriteLine(Log.LogLevels.Warning, "Player should be dead -> no request from client !!!");
        //        return;
        //    }

        //    Log.WriteLine(Log.LogLevels.Debug, "Player {0} make {1}", player.Player.Username, actionType);
        //    switch (actionType)
        //    {
        //        case ActionType.MoveUp:
        //            MovePlayer(player.Player, 0, -1, actionType);
        //            break;
        //        case ActionType.MoveDown:
        //            MovePlayer(player.Player, 0, +1, actionType);
        //            break;
        //        case ActionType.MoveRight:
        //            MovePlayer(player.Player, +1, 0, actionType);
        //            break;
        //        case ActionType.MoveLeft:
        //            MovePlayer(player.Player, -1, 0, actionType);
        //            break;
        //        case ActionType.DropBomb:
        //            DropBomb(player.Player);
        //            break;
        //        //todo 
        //        //case ActionType.ShootBomb:
        //        //    ShootBomb(player.player);
        //        //    break;
        //    }
        //}
        ////OKAY
        //private void MovePlayer(Player player, int stepX, int stepY, ActionType actionType)
        //{
        //    // Get object at future player location
        //    LivingObject collider = GameCreated.Map.LivingObjects.FirstOrDefault(x => player.Position.Y + stepY == x.Position.Y
        //                                                                              && player.Position.X + stepX == x.Position.X);
        //    // Can't go thru wall or bomb
        //    if (collider != null && (collider is Wall || collider is Bomb))
        //    {
        //        Log.WriteLine(Log.LogLevels.Debug, "Collider found : {0}, {1} type {2}",collider.Position.X, collider.Position.Y, collider);
        //        return;
        //    }
        //    //remove player from the map
        //    GameCreated.Map.LivingObjects.Remove(player);

        //    Position newPosition = new Position
        //    {
        //        X = player.Position.X + stepX,
        //        Y = player.Position.Y + stepY
        //    };
        //    Log.WriteLine(Log.LogLevels.Debug, "Player {0} move from {1},{2} to {3},{4}", player.Username, player.Position.X, player.Position.Y, newPosition.X,newPosition.Y);
        //    // Send new player position to players
        //    ExceptionFreeAction(PlayersOnline, playerModel => playerModel.CallbackService.OnPlayerMove(player, newPosition, actionType));
        //    //update position's player
        //    player.Position.Y += stepY;
        //    player.Position.X += stepX;
        //    //add player to the global map
        //    GameCreated.Map.LivingObjects.Add(player);
        //}
        ////OKAY
        //private void DropBomb(Player player)
        //{
        //    if (player == null)
        //    {
        //        Log.WriteLine(Log.LogLevels.Error, "player is null => WTF ??");
        //        return;
        //    }
               
        //    Log.WriteLine(Log.LogLevels.Debug, "Player {0} wants to drop a bomb.", player.Username);
        //    int count = GameCreated.Map.LivingObjects.Count(x => x is Bomb && ((Bomb) x).PlayerId == player.ID);
        //    //if player already have the max bomb number on the battle field
        //    if (count >= player.BombNumber)
        //    {
        //        Log.WriteLine(Log.LogLevels.Warning, "Player's current bomb on field : {0}. Max authorized is {1}", count , player.BombNumber);
        //        return;
        //    }
        //    //otherwise create a new bomb
        //    Bomb newBomb = new Bomb
        //    {
        //        ID = IdCount++,
        //        PlayerId = player.ID,
        //        Power = player.BombPower,
        //        Position = new Position
        //        {
        //            X = player.Position.X,
        //            Y = player.Position.Y
        //        }
        //    };
        //    //add the new bomb created into the map
        //    Log.WriteLine(Log.LogLevels.Debug, "Bomb dropped by {0}",player.Username);
        //    GameCreated.Map.LivingObjects.Add(newBomb);
        //    //warn players to be aware ofthat new bomb
        //    ExceptionFreeAction(PlayersOnline, playerModel => playerModel.CallbackService.OnBombDropped(newBomb));
        //    //make the bomb explode after 1,5 sec
        //    // OLD CODE: new Timer(BombExplode, newBomb, 1500, Timeout.Infinite);
        //    // NEW CODE: it's just a POC
        //    Timer t = new Timer(BombExplode, newBomb, 3000, Timeout.Infinite);
        //    _timers.Add(t);
        //}
        ////OKAY
        //private void BombExplode(object bomb)
        //{
        //    Log.WriteLine(Log.LogLevels.Debug, "Bomb explode {0}", bomb);

        //    Bomb bombToExplode = bomb as Bomb;

        //    //Not supposed to happen
        //    if (bombToExplode == null)
        //    {
        //        Log.WriteLine(Log.LogLevels.Error, "Bomb is null => WTF ??" );
        //        return;
        //    }

        //    List<LivingObject> impacted = new List<LivingObject>();
        //    List<LivingObject> tempList;
        //    //research impact of objects in 4 directions
        //    for (int direction = 0; direction < 4; direction++)
        //    {
        //        //handle bomb power
        //        for (int i = 1; i <= bombToExplode.Power; i++)
        //        {
        //            tempList = new List<LivingObject>();

        //            switch (direction)
        //            {
        //                    //up
        //                case 0:
        //                    tempList.AddRange(
        //                        GameCreated.Map.LivingObjects.Where(
        //                            livingObject => livingObject.Position.Y == bombToExplode.Position.Y - i
        //                                 && livingObject.Position.X == bombToExplode.Position.X).ToList());
        //                    CheckBomb(tempList);
        //                    break;
        //                    //down
        //                case 1:
        //                    tempList.AddRange(
        //                        GameCreated.Map.LivingObjects.Where(
        //                            livingObject => livingObject.Position.Y == bombToExplode.Position.Y + i
        //                                 && livingObject.Position.X == bombToExplode.Position.X).ToList());
        //                    CheckBomb(tempList);
        //                    break;
        //                    //left
        //                case 2:
        //                    tempList.AddRange(
        //                        GameCreated.Map.LivingObjects.Where(
        //                            livingObject => livingObject.Position.X == bombToExplode.Position.X - i
        //                                 && livingObject.Position.Y == bombToExplode.Position.Y).ToList());
        //                    CheckBomb(tempList);
        //                    break;
        //                    //right
        //                default:
        //                    tempList.AddRange(
        //                        GameCreated.Map.LivingObjects.Where(
        //                            livingObject => livingObject.Position.X == bombToExplode.Position.X + i
        //                                 && livingObject.Position.Y == bombToExplode.Position.Y).ToList());
        //                    CheckBomb(tempList);
        //                    break;
        //            }
        //            //if we encountered an empty space
        //            if (tempList.Count == 0)
        //                continue;
        //            //if we encountered a wall undestructible don't need to go further in the current direction
        //            if (!IsUndestructible(tempList))
        //                break;
        //            impacted.AddRange(tempList);

        //        }
        //    }
        //    //todo:check if the players'bomb doesn't move
        //    tempList = new List<LivingObject>();

        //    //objects at the same place than the bomb
        //    tempList.AddRange(GameCreated.Map.LivingObjects
        //        .Where(x => x.ID == bombToExplode.ID
        //                    && x.Position.X == bombToExplode.Position.X
        //                    && x.Position.Y == bombToExplode.Position.Y).ToList());
            
        //    impacted.AddRange(tempList);

        //    foreach (LivingObject livingObject in impacted)
        //    {
        //        Log.WriteLine(Log.LogLevels.Debug, "Object destroyed : {0}", livingObject);
        //    }

        //    if (!impacted.Any()) return;
        //    //remove impacted object at the end
        //    GameCreated.Map.LivingObjects.RemoveAll(impacted.Contains);
        //    //handle all objects
        //    HandleImpact(bombToExplode, impacted);
        //}

        //private void HandleImpact(Bomb bombToExplode, List<LivingObject> impactedObjects)
        //{
        //    if (bombToExplode == null || impactedObjects == null)
        //        return;
        //    //warn all players that a bomb exploded
        //    ExceptionFreeAction(PlayersOnline, playerModel => playerModel.CallbackService.OnBombExploded(bombToExplode, impactedObjects));
        //    // foreach impacted objects except the bomb that exploded
        //    foreach (var impactedObject in impactedObjects.Where(x=>x.ID != bombToExplode.ID))
        //    {
        //        if (impactedObject is Player)
        //            HandleImpactedPlayer(impactedObject as Player);
        //        if(impactedObject is Bomb)
        //            BombExplode(impactedObject as Bomb);
        //    }   
        //}
        ////todo check real use of this method
        //private void CheckBomb(List<LivingObject> tempList)
        //{
        //    foreach (LivingObject livingObject in tempList)
        //    {
        //        if (!(livingObject is Bomb)) 
        //            continue;
        //        LivingObject o = livingObject;
        //        foreach (LivingObject o2 in tempList)
        //        {
        //            if (o.Position.X == o2.Position.X && o.Position.Y == o2.Position.Y)
        //                GameCreated.Map.LivingObjects.Remove(o2);
        //        }
                    
        //        BombExplode(livingObject);
        //    }
        //}
        ////todo test more deeply (Question : if wall destructible => check or not if there is someone behind ?)
        //private static bool IsUndestructible(IEnumerable<LivingObject> list)
        //{
        //    return list.Any(livingObject => (livingObject is Wall) && ((Wall)livingObject).WallType == WallType.Undestructible);
        //}
        ////OKAY
        //private void HandleImpactedPlayer(Player impactedPlayer)
        //{
        //    var player = PlayersOnline.First(x => x.Player.ID == (impactedPlayer).ID);

        //    if (player == null)
        //    {
        //        Log.WriteLine(Log.LogLevels.Error, "Player impacted not found in players online disco Maybe ?");
        //        return;
        //    }

        //    //  set alive to false
        //    player.Alive = false;
        //    //  send LOST
        //    player.CallbackService.OnMyDeath();
        //    // if one player alive left, send WON to winner + RESTART + change status of the game
        //    if (PlayersOnline.Count(x => x.Alive) == 1)
        //    {
        //        GameCreated.CurrentStatus = GameStatus.Stopped;
        //        ExceptionFreeAction(PlayersOnline.FirstOrDefault(x => x.Alive), playerModel => playerModel.CallbackService.OnWin());
        //        ExceptionFreeAction(PlayersOnline.FirstOrDefault(x => x.Player.IsCreator), playerModel => playerModel.CallbackService.OnCanRestartGame());
        //    }
        //    // if no player alive left, send DRAW to everyone + RESTART + change status of the game
        //    if (PlayersOnline.All(x => !x.Alive))
        //    {
        //        GameCreated.CurrentStatus = GameStatus.Stopped;
        //        ExceptionFreeAction(PlayersOnline, playerModel => playerModel.CallbackService.OnDraw());
        //        ExceptionFreeAction(PlayersOnline.FirstOrDefault(x => x.Player.IsCreator), playerModel => playerModel.CallbackService.OnCanRestartGame());
        //    }
        //}
    }

    public enum ServerStatus
    {
        Started,
        Stopped
    }
}
