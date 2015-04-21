using System;
using System.CodeDom.Compiler;
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
        public ICallbackManager CallbackManager { get; private set; }
        public IGameManager GameManager { get; private set; }
        public IUserManager UserManager { get; set; }
        public IMapManager MapManager { get; set; }

        //OKAY
        internal void Initialize()
        {
            ServerStatus = ServerStatus.Started;
            UserManager = new UserManager();
            CallbackManager = new CallbackManager(UserManager);
            MapManager = new MapManager(UserManager);
            GameManager = new GameManager(MapManager);

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
            CallbackManager.SendUsernameListToNewUser(newUser);
            if(UserManager.GetNumberOfUsers() > 1)
                //Warning players that a new player is connected by sending them the list of all players online
                CallbackManager.SendUsernameListToAllOtherUserAfterConnection(newUser);
        }
        //OKAY
        public void DisconnectUser(Guid ID)
        {
            //if username is empty
            if (ID == Guid.Empty)
            {
                string errorMessage = string.Format("Empty ID.");
                Log.WriteLine(Log.LogLevels.Info, errorMessage);
                return;
            }

            User userToDelete = UserManager.GetUserById(ID);

            if (userToDelete == null)
            {
                string errorMessage = string.Format("Unknown ID.");
                Log.WriteLine(Log.LogLevels.Info, errorMessage);
                return;    
            }

            UserManager.DeleteUser(userToDelete);

            Log.WriteLine(Log.LogLevels.Info, "User deleted : {0}", userToDelete.Username);

            //Warning players that a new player is connected by sending them the list of all players online
            CallbackManager.SendUsernameListToAllOtherUserAfterDisconnection(userToDelete);

        }
        //OKAY
        public void StartNewGame()
        {
            //Create a new game
            GameManager.CreateNewGame();

            if (GameManager.GetCurrentGame() == null)
            {
                string errorMessage = "Problem creating the new game";
                Log.WriteLine(Log.LogLevels.Error, errorMessage);
                return;
            }

            Log.WriteLine(Log.LogLevels.Debug, "New Game created");

            //send the game to all players (only once)
            CallbackManager.SendGameToAllUsers(GameManager.GetCurrentGame());
        }
        //TODO for bonuses
        public void PlayerAction(Guid ID, ActionType actionType)
        {
            //retreive the player who made the action
            User user = UserManager.GetUserById(ID);

            //not supposed to happend but okay
            if (GameManager.GetCurrentGame().CurrentStatus == GameStatus.Stopped)
            {
                Log.WriteLine(Log.LogLevels.Warning, "Game stopped -> no request from client !!!");
                return;
            }
            //not supposed to happend not okay at all
            if (user == null)
            {
                Log.WriteLine(Log.LogLevels.Error, "Player who made the action is null... shouldn't be !!!");
                return;
            }
            //not supposed to happend but okay
            if (!user.Player.Alive)
            {
                Log.WriteLine(Log.LogLevels.Warning, "Player should be dead -> no request from client !!!");
                return;
            }
            Log.WriteLine(Log.LogLevels.Debug, "Player {0} make {1}", user.Username, actionType);

            Position position = new Position();
            switch (actionType)
            {
                case ActionType.MoveUp:
                    position = MovePlayer(user, 0, -1, actionType);
                    
                    break;
                case ActionType.MoveDown:
                    position = MovePlayer(user, 0, +1, actionType);
                    break;
                case ActionType.MoveRight:
                    position = MovePlayer(user, +1, 0, actionType);
                    break;
                case ActionType.MoveLeft:
                    position = MovePlayer(user, -1, 0, actionType);
                    break;
                case ActionType.DropBomb:
                    //DropBomb(player.Player);
                    break;
                //todo 
                //case ActionType.ShootBomb:
                //    ShootBomb(player.player);
                //    break;
            }
            if (position == null)
                return;

            CallbackManager.SendMoveToAllUsers(user, position);
        }
        ////OKAY
        private Position MovePlayer(User user, int stepX, int stepY, ActionType actionType)
        {
           return MapManager.MovePlayer(user, stepX, stepY);
        }
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
