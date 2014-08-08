﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bomberman.Common;
using Bomberman.Common.DataContracts;
using Bomberman.Server.Console.Interfaces;

// BUG:
//  bomb move, if they hit a wall before explosion time they stay against wall without exploding
//  -> BIG BAD problem with timeout list handling

namespace Bomberman.Server.Console
{
    public enum ServerStates
    {
        WaitStartGame,
        GameStarted,
    }

    public class Server : IServerMapInteraction
    {
        private const int MaxExplosionRange = 1;
        private const int BombTimer = 3000; // in ms
        private const int FlameTimer = 500; // in ms
        private const int BombMoveTimer = 500; // in ms

        private static readonly int[] BombNeighboursX = {-1, 1, 0, 0};
        private static readonly int[] BombNeighboursY = {0, 0, -1, 1};

        private readonly WCFHost _host;
        private readonly IPlayerManager _playerManager;
        private readonly IMapManager _mapManager;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _actionTask;
        private readonly BlockingCollection<Action> _gameActionBlockingCollection = new BlockingCollection<Action>(new ConcurrentQueue<Action>());

        private readonly CellMap _cellMap; // TODO: use this instead of GameMap and timer tables

        public ServerStates State { get; private set; }
        //public Map GameMap { get; private set; }
        public int PlayersInGameCount { get; private set; }

        public Server(WCFHost host, IPlayerManager playerManager, IMapManager mapManager)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            if (playerManager == null)
                throw new ArgumentNullException("playerManager");

            _host = host;
            _playerManager = playerManager;
            _mapManager = mapManager;
            _cellMap = new CellMap(this, playerManager);

            _host.OnLogin += OnLogin;
            _host.OnLogout += OnLogout;
            _host.OnStartGame += OnStartGame;
            _host.OnMove += OnMove;
            _host.OnPlaceBomb += OnPlaceBomb;
            _host.OnChat += OnChat;

            State = ServerStates.WaitStartGame;
        }

        public void Start()
        {
            _host.Start();

            _cancellationTokenSource = new CancellationTokenSource();
            _actionTask = new Task(GameActionTask, _cancellationTokenSource.Token);
            _actionTask.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _host.Stop();
            _cellMap.Stop();
            _actionTask.Wait(1000);
        }

        private void OnLogin(IPlayer player, int playerId)
        {
            Log.WriteLine(Log.LogLevels.Info, "New player {0}|{1} connected", player.Name, playerId);

            EntityTypes playerEntity = EntityTypes.Empty;
            switch (playerId)
            {
                case 0:
                    playerEntity = EntityTypes.Player1;
                    break;
                case 1:
                    playerEntity = EntityTypes.Player2;
                    break;
                case 2:
                    playerEntity = EntityTypes.Player3;
                    break;
                case 3:
                    playerEntity = EntityTypes.Player4;
                    break;
            }
            player.PlayerEntity = playerEntity;

            // Inform player about its playerId
            player.OnLogin(LoginResults.Successful, playerId, playerEntity, _mapManager.MapDescriptions);

            // Inform other player about new player
            foreach (IPlayer other in _playerManager.Players.Where(x => x != player))
                other.OnUserConnected(player.Name, playerId);

            // TODO: if game is started, handle differently
        }

        private void OnLogout(IPlayer player)
        {
            throw new NotImplementedException();
        }

        private void OnStartGame(IPlayer player, int mapId)
        {
            Log.WriteLine(Log.LogLevels.Info, "Start game {0} map {1}", player.Name, mapId);

            if (State == ServerStates.WaitStartGame)
            {
                // Reset action list
                while (_gameActionBlockingCollection.Count > 0)
                {
                    Action item;
                    _gameActionBlockingCollection.TryTake(out item);
                }

                // Generate cell map
                Map map = _mapManager.Maps.FirstOrDefault(x => x.Description.Id == mapId);
                if (map != null)
                {
                    // Clone map
                    Map clonedMap = new Map
                        {
                            Description = map.Description,
                            MapAsArray = (EntityTypes[]) map.MapAsArray.Clone()
                        };

                    bool positionError = false;
                    // Set players position
                    foreach (IPlayer p in _playerManager.Players)
                    {
                        var entry = clonedMap.MapAsArray.Select((entity, index) => new
                            {
                                entity,
                                index
                            }).FirstOrDefault(x => x.entity == p.PlayerEntity);
                        if (entry != null)
                        {
                            // Set player position
                            int x, y;
                            clonedMap.GetLocation(entry.index, out x, out y);
                            p.LocationX = x;
                            p.LocationY = y;
                        }
                        else
                        {
                            Log.WriteLine(Log.LogLevels.Error, "Cannot find position of {0} in map {1}", player.PlayerEntity, clonedMap.Description.Id);
                            positionError = true;
                            break;
                        }
                    }

                    // Replace unused player with dust
                    for (int i = 0; i < clonedMap.MapAsArray.Length; i++)
                    {
                        EntityTypes entity = clonedMap.MapAsArray[i];
                        if (IsPlayer(entity))
                        {
                            EntityTypes playerEntity = GetPlayer(entity);
                            if (playerEntity != EntityTypes.Empty)
                            {
                                // Search player in player list
                                bool playerFound = _playerManager.Players.Any(x => x.PlayerEntity == playerEntity);
                                if (!playerFound)
                                {
                                    Log.WriteLine(Log.LogLevels.Debug, "Replace unused player position {0} with dust at index {1}", playerEntity, i);
                                    clonedMap.MapAsArray[i] = EntityTypes.Dust;
                                }
                            }
                            else
                                Log.WriteLine(Log.LogLevels.Warning, "Inconsistant player position at index {0} in map {1}", i, clonedMap.Description.Id);
                        }
                    }

                    // Initialize cell map
                    _cellMap.Initialize(clonedMap);

                    // If no problem while positioning player, let's go
                    if (!positionError)
                    {
                        // Change player state
                        foreach (IPlayer p in _playerManager.Players)
                            p.State = PlayerStates.Playing;

                        //
                        PlayersInGameCount = _playerManager.Players.Count(x => x.State == PlayerStates.Playing);

                        // Inform players about game started
                        foreach (IPlayer p in _playerManager.Players)
                            p.OnGameStarted(p.LocationX, p.LocationY, clonedMap);

                        State = ServerStates.GameStarted;
                    }
                    else
                        Log.WriteLine(Log.LogLevels.Error, "Game not started, players position not set");
                }
                else
                    Log.WriteLine(Log.LogLevels.Warning, "Map {0} doesn't exist", mapId);
            }
            else
                Log.WriteLine(Log.LogLevels.Warning, "Game already started");
        }

        private void OnMove(IPlayer player, Directions direction)
        {
            Log.WriteLine(Log.LogLevels.Info, "Move {0}:{1}", player.Name, direction);

            if (State == ServerStates.GameStarted && player.State == PlayerStates.Playing)
                EnqueueGameAction(() => MovePlayerAction(player, direction));
        }

        private void OnPlaceBomb(IPlayer player)
        {
            Log.WriteLine(Log.LogLevels.Info, "OnPlaceBomb {0}", player.Name);

            if (State == ServerStates.GameStarted && player.State == PlayerStates.Playing)
                EnqueueGameAction(() => PlaceBombAction(player));
        }

        private void OnChat(IPlayer player, string msg)
        {
            Log.WriteLine(Log.LogLevels.Info, "Chat from {0}:{1}", player.Name, msg);

            // Send message to other players
            int id = _playerManager.GetId(player);
            foreach (IPlayer other in _playerManager.Players.Where(x => x != player))
                other.OnChatReceived(id, msg);
        }

        //
        private void CheckDeathsAndWinnerOrDraw()
        {
            // Inform dying player and other about dying player
            foreach (IPlayer player in _playerManager.Players.Where(p => p.State == PlayerStates.Dying))
            {
                // Inform player he/she's dead
                player.OnGameLost();

                // Inform other player about player's death
                int playerId = _playerManager.GetId(player);
                IPlayer player1 = player;
                foreach (IPlayer other in _playerManager.Players.Where(x => x != player1))
                    other.OnKilled(playerId);
            }

            //  if no player playing -> draw
            //  if 1 player playing -> winner
            int playingCount = _playerManager.Players.Count(x => x.State == PlayerStates.Playing);
            if (playingCount == 0)
            {
                Log.WriteLine(Log.LogLevels.Info, "Game ended in a DRAW");
                // Inform players about draw
                foreach (IPlayer player in _playerManager.Players)
                    player.OnGameDraw();
                // Change server state
                State = ServerStates.WaitStartGame;
            }
            else if (playingCount == 1 && PlayersInGameCount > 1) // Solo game doesn't stop when only one player left
            {
                IPlayer winner = _playerManager.Players.First(x => x.State == PlayerStates.Playing);
                int id = _playerManager.GetId(winner);

                Log.WriteLine(Log.LogLevels.Info, "Player {0}|{1} WON", winner.Name, id);
                // Inform players about winner
                foreach (IPlayer player in _playerManager.Players)
                    player.OnGameWon(id);
                // Change server state
                State = ServerStates.WaitStartGame;
            }

            // Change dying to died
            foreach (IPlayer p in _playerManager.Players)
                if (p.State == PlayerStates.Dying)
                {
                    int id = _playerManager.GetId(p);
                    Log.WriteLine(Log.LogLevels.Debug, "Kill definitively {0}|{1}", p.Name, id);
                    p.State = PlayerStates.Dead;
                }
        }

        // Actions
        private void MovePlayerAction(IPlayer player, Directions direction)
        {
            // Get old coordinates
            int oldLocationX = player.LocationX;
            int oldLocationY = player.LocationY;

            // Search player in cell map
            Entity playerEntity = _cellMap.GetEntity(player.PlayerEntity, oldLocationX, oldLocationY);

            // Get new coordinates
            int stepX, stepY;
            GetDirectionSteps(direction, out stepX, out stepY);
            int newLocationX = ComputeLocation(oldLocationX, stepX);
            int newLocationY = ComputeLocation(oldLocationY, stepY);

            // Check if collider on new location
            Cell colliderCell = _cellMap.GetCell(newLocationX, newLocationY);
            if (colliderCell.All(e => e.IsEmpty)) // can only move to empty location
            {
                Log.WriteLine(Log.LogLevels.Debug, "Moved successfully from {0},{1} to {2},{3}", oldLocationX, oldLocationY, newLocationX, newLocationY);

                // Set new location
                player.LocationX = newLocationX;
                player.LocationY = newLocationY;

                // Inform player about its new location
                player.OnMoved(true, oldLocationX, oldLocationY, newLocationX, newLocationY);

                // TODO: get bonus if any

                // Move player on map
                _cellMap.MoveEntity(playerEntity, newLocationX, newLocationY);
            }
            else if (colliderCell.Any(e => e.IsBomb))
            {
                Log.WriteLine(Log.LogLevels.Debug, "Push bomb at {0},{1}", newLocationX, newLocationY);

                BombEntity bomb = colliderCell.FirstOrDefault(x => x.IsBomb) as BombEntity;
                if (bomb != null)
                {
                    if (!bomb.IsMoving)
                    {
                        // Init move
                        bomb.InitMove(direction, BombMoveTimer);
                        // Perform move action immediately
                        MoveBombAction(bomb);
                    }
                    else
                        Log.WriteLine(Log.LogLevels.Debug, "Cannot push bomb because its already moving");
                }
                else
                    Log.WriteLine(Log.LogLevels.Error, "Pushed bomb doesn't exist anymore at this location {0},{1}", newLocationX, newLocationY);
            }
            else if (colliderCell.Any(e => e.IsFlames)) // dead
            {
                Log.WriteLine(Log.LogLevels.Debug, "Moved successfully from {0},{1} to {2},{3} but died because of Flames", oldLocationX, oldLocationY, newLocationX, newLocationY);

                // Kill player
                player.State = PlayerStates.Dying;

                // Set new location
                player.LocationX = newLocationX;
                player.LocationY = newLocationY;

                // Inform player about its new location
                player.OnMoved(true, oldLocationX, oldLocationY, newLocationX, newLocationY);

                // Move player on map
                _cellMap.MoveEntity(playerEntity, newLocationX, newLocationY);

                // Check deaths, winner or draw
                CheckDeathsAndWinnerOrDraw();
            }
            else
            {
                string collider = colliderCell.Select(x => x.Type.ToString()).Aggregate((s, s1) => s + s1);
                Log.WriteLine(Log.LogLevels.Debug, "Moved from {0},{1} to {2},{3} failed because of collider {4}", oldLocationX, oldLocationY, newLocationX, newLocationY, collider);
                player.OnMoved(false, -1, -1, -1, -1);
            }
        }

        private void MoveBombAction(BombEntity bomb)
        {
            //  If collider is Player or Flame (other collider only forbids move)
            //      Move
            //      Inform players about move
            //      Explosion
            //  Else, (no collider)
            //      Move
            //      Inform players about move
            //      Reset move timer
            //      Insert bomb in timed action queue

            // Try to move bomb
            int toX, toY;
            _cellMap.ComputeNewCoordinates(bomb, bomb.Direction, out toX, out toY);
            // Check collider
            Cell destinationCell = _cellMap.GetCell(toX, toY);
            if (destinationCell.Any(x => x.IsPlayer || x.IsFlames)) // collision with player or flame -> explosion
            {
                // Move
                _cellMap.MoveEntity(bomb, toX, toY);
                // Explosion
                OnExplosion(bomb.X, bomb.Y, bomb.Range);
            }
            else if (destinationCell.All(x => x.IsEmpty))
            {
                // Remove bomb from timed action queue
                _cellMap.RemoveTimeoutEntity(bomb);
                // Move
                _cellMap.MoveEntity(bomb, toX, toY);
                // Reset move timer
                bomb.InitMove(bomb.Direction, bomb.MoveDelay);
                // Insert bomb in timed action queue
                _cellMap.AddTimeoutEntity(bomb);
            }
        }

        private void PlaceBombAction(IPlayer player)
        {
            // TODO: check player max bomb
            // TODO: can't place 2 bombs at the same place or a bonus+bomb
            // TODO: if place a bomb in flames -> immediate explosion

            int bombLocationX = player.LocationX;
            int bombLocationY = player.LocationY;

            // Create bomb
            BombEntity bombEntity = new BombEntity(player, bombLocationX, bombLocationY, MaxExplosionRange, BombTimer);

            // Inform player about bomb placement
            player.OnBombPlaced(true, EntityTypes.Bomb, bombLocationX, bombLocationY);

            // Add bomb to map
            _cellMap.AddEntity(bombEntity);

            Log.WriteLine(Log.LogLevels.Debug, "Bomb placed by {0} at {1},{2}: {3}", player.Name, bombLocationX, bombLocationY, EntityTypes.Bomb);
        }

        private void EnqueueGameAction(Action action)
        {
            _gameActionBlockingCollection.Add(action);
        }

        private void GameActionTask()
        {
            while (true)
            {
                try
                {
                    Action action;
                    bool taken = _gameActionBlockingCollection.TryTake(out action, 10, _cancellationTokenSource.Token);
                    if (taken)
                    {
                        try
                        {
                            Log.WriteLine(Log.LogLevels.Debug, "Dequeue, item in queue {0}", _gameActionBlockingCollection.Count);
                            action();
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(Log.LogLevels.Error, "Exception raised in GameActionsTask. Exception:{0}", ex);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.WriteLine(Log.LogLevels.Info, "Taking cancelled");
                    break;
                }
            }
        }

        // Helpers
        private static void GetDirectionSteps(Directions direction, out int stepX, out int stepY)
        {
            stepX = 0;
            stepY = 0;
            switch (direction)
            {
                case Directions.Left:
                    stepX = -1;
                    break;
                case Directions.Right:
                    stepX = +1;
                    break;
                case Directions.Up:
                    stepY = -1;
                    break;
                case Directions.Down:
                    stepY = +1;
                    break;
            }
        }

        private int ComputeLocation(int coord, int step)
        {
            coord = (coord + step)%_cellMap.Size;
            if (coord < 0)
                coord += _cellMap.Size;
            return coord;
        }

        private static bool IsPlayer(EntityTypes entity)
        {
            return (entity & EntityTypes.Player1) == EntityTypes.Player1
                   || (entity & EntityTypes.Player2) == EntityTypes.Player2
                   || (entity & EntityTypes.Player3) == EntityTypes.Player3
                   || (entity & EntityTypes.Player4) == EntityTypes.Player4;
        }

        private static EntityTypes GetPlayer(EntityTypes entity)
        {
            if ((entity & EntityTypes.Player1) == EntityTypes.Player1)
                return EntityTypes.Player1;
            if ((entity & EntityTypes.Player2) == EntityTypes.Player2)
                return EntityTypes.Player2;
            if ((entity & EntityTypes.Player3) == EntityTypes.Player3)
                return EntityTypes.Player3;
            if ((entity & EntityTypes.Player4) == EntityTypes.Player4)
                return EntityTypes.Player4;
            return EntityTypes.Empty;
        }

        #region IServerMapInteraction

        public bool IsGameStarted
        {
            get { return State == ServerStates.GameStarted; }
        }

        public void MoveBomb(BombEntity bomb)
        {
            EnqueueGameAction(() => MoveBombAction(bomb));
        }

        public void OnEntityAddedInMap(EntityTypes type, int x, int y)
        {
            foreach (IPlayer player in _playerManager.Players)
                player.OnEntityAdded(type, x, y);
        }

        public void OnEntityDeletedInMap(EntityTypes type, int x, int y)
        {
            foreach (IPlayer player in _playerManager.Players)
                player.OnEntityDeleted(type, x, y);
        }

        public void OnEntityMovedInMap(EntityTypes type, int fromX, int fromY, int toX, int toY)
        {
            foreach (IPlayer player in _playerManager.Players)
                player.OnEntityMoved(type, fromX, fromY, toX, toY);
        }

        public void OnExplosion(int x, int y, int range)
        {
            List<MapModification> modifications = GenerateExplosionModifications(x, y, true, range);

            if (modifications.Any())
            {
                foreach (IPlayer player in _playerManager.Players)
                    player.OnMapModified(modifications);
            }

            CheckDeathsAndWinnerOrDraw();
        }

        private void AddExplosionModification(List<MapModification> list, Entity entity, MapModificationActions action)
        {
            switch (action)
            {
                case MapModificationActions.Add:
                    _cellMap.AddEntity(entity);
                    break;
                case MapModificationActions.Delete:
                    _cellMap.RemoveEntity(entity);
                    break;
            }
            MapModification modification = new MapModification
            {
                X = entity.X,
                Y = entity.Y,
                Entity = entity.Type,
                Action = action
            };
            list.Add(modification);
        }

        private List<MapModification> GenerateExplosionModifications(int x, int y, bool addFlames, int explosionRange)
        {
            List<MapModification> modifications = new List<MapModification>();

            Log.WriteLine(Log.LogLevels.Debug, "Explosion at {0}, {1}", x, y);

            Cell cell = _cellMap.GetCell(x, y);

            // Destroy dust
            if (cell.Any(e => e.IsDust))
            {
                foreach (Entity entity in cell.Where(e => e.IsDust))
                {
                    Log.WriteLine(Log.LogLevels.Debug, "Dust removed at {0},{1}", x, y);
                    AddExplosionModification(modifications, entity, MapModificationActions.Delete);
                }
            }

            // Kill player
            if (cell.Any(e => e.IsPlayer))
            {
                foreach (Entity entity in cell.Where(e => e.IsPlayer))
                {
                    IPlayer player = _playerManager.Players.FirstOrDefault(p => p.PlayerEntity == entity.Type);
                    if (player != null)
                    {
                        int id = _playerManager.GetId(player);
                        Log.WriteLine(Log.LogLevels.Info, "Player {0}|{1} dead due to explosion at {2},{3}: {4}", player.Name, id, x, y, entity);
                        // Kill player (will be killed definitively when every explosion modification will be done)
                        player.State = PlayerStates.Dying;
                    }
                    else
                        Log.WriteLine(Log.LogLevels.Error, "Dying player not found in player list at {0},{1}: {2}", x, y, entity);
                }
            }

            if (addFlames)
            {
                // Create flame
                FlameEntity flame = new FlameEntity(x, y, FlameTimer);
                AddExplosionModification(modifications, flame, MapModificationActions.Add);

                Log.WriteLine(Log.LogLevels.Debug, "Flame at {0},{1}", x, y);
            }

            if (cell.Any(e => e.IsBomb)) // Bomb found, check neighbourhood
            {
                Entity bombEntity = cell.FirstOrDefault(e => e.IsBomb);
                //foreach (Entity bombEntity in cell.Where(e => e.IsBomb))
                if( bombEntity != null)
                {
                    Log.WriteLine(Log.LogLevels.Debug, "Bomb at {0},{1}", x, y);

                    // Remove bombs
                    Log.WriteLine(Log.LogLevels.Debug, "Remove bomb at {0},{1}", x, y);
                    AddExplosionModification(modifications, bombEntity, MapModificationActions.Delete);

                    // Check explosion in surrounding cell and remove cell
                    for (int neighbourIndex = 0; neighbourIndex < BombNeighboursX.Length; neighbourIndex++)
                        for (int range = 1; range <= explosionRange; range++)
                        {
                            int stepX = BombNeighboursX[neighbourIndex] * range;
                            int stepY = BombNeighboursY[neighbourIndex] * range;
                            int neighbourX = ComputeLocation(x, stepX);
                            int neighbourY = ComputeLocation(y, stepY);

                            // Stop explosion propagation if wall found
                            Cell neighbourCell = _cellMap.GetCell(neighbourX, neighbourY);
                            if (neighbourCell.Any(e => e.IsWall))
                            {
                                Log.WriteLine(Log.LogLevels.Debug, "Stop propagating explosion {0},{1} -> {2},{3} wall found", x, y, neighbourX, neighbourY);
                                break;
                            }

                            Log.WriteLine(Log.LogLevels.Debug, "Propagating explosion to neighbourhood {0},{1} -> {2},{3}", x, y, neighbourX, neighbourY);

                            List<MapModification> neighbourModifications = GenerateExplosionModifications(neighbourX, neighbourY, addFlames, range);
                            modifications.AddRange(neighbourModifications);
                        }
                    Log.WriteLine(Log.LogLevels.Debug, "Bomb completed at {0},{1}", x, y);
                }
            }

            return modifications;
        }

        #endregion
    }

    public interface ITimedEntity
    {
        bool IsTimeoutElapsed(DateTime now);
        void TimeoutAction(DateTime now, CellMap map);
    }

    public class Entity
    {
        public EntityTypes Type { get; set; }
        public int X { get; set; } // coordinates are stored to speed-up cell search from entity
        public int Y { get; set; }

        public Entity(EntityTypes type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        public bool IsPlayer
        {
            get
            {
                return Type == EntityTypes.Player1
                       || Type  == EntityTypes.Player2
                       || Type  == EntityTypes.Player3
                       || Type == EntityTypes.Player4;
            }
        }

        public bool IsFlames
        {
            get { return Type == EntityTypes.Flames; }
        }

        public bool IsEmpty
        {
            get { return Type == EntityTypes.Empty; }
        }

        public bool IsDust
        {
            get { return Type == EntityTypes.Dust; }
        }

        public bool IsBomb
        {
            get { return Type == EntityTypes.Bomb; }
        }

        public bool IsWall
        {
            get { return Type == EntityTypes.Wall; }
        }
    }

    public class BombEntity : Entity, ITimedEntity
    {
        public IPlayer Player { get; private set; }

        public int Range { get; private set; }
        public DateTime ExplosionRemainingTimeout { get; private set; } // in ms

        public bool IsMoving { get; private set; }
        public Directions Direction { get; private set; }
        public int MoveDelay { get; private set; }
        public DateTime NextMoveRemainingTimeout { get; private set; } // in ms

        public BombEntity(IPlayer player, int x, int y, int range, int delayInMs)
            : base(EntityTypes.Bomb, x, y)
        {
            Player = player;
            Range = range;
            ExplosionRemainingTimeout = DateTime.Now.AddMilliseconds(delayInMs);
        }

        public void InitMove(Directions direction, int delayInMs)
        {
            MoveDelay = delayInMs;
            NextMoveRemainingTimeout = DateTime.Now.AddMilliseconds(delayInMs);
            if (!IsMoving) // Cannot change direction if already moving
            {
                Direction = direction;
                IsMoving = true;
            }
        }

        #region ITimedEntity
        
        public bool IsTimeoutElapsed(DateTime now)
        {
            return ExplosionRemainingTimeout <= now || (IsMoving && NextMoveRemainingTimeout <= now);
        }

        public void TimeoutAction(DateTime now, CellMap map)
        {
            // Handle explosion first
            if (ExplosionRemainingTimeout <= now)
            {
                Log.WriteLine(Log.LogLevels.Debug, "Bomb explosion timeout reached");
                map.ExplosionAction(X, Y, Range);
            }
            // Then move if no explosion
            else if (IsMoving && NextMoveRemainingTimeout <= now)
            {
                Log.WriteLine(Log.LogLevels.Debug, "Bomb move timeout reached -> queue move action from {0},{1} direction {2}", X, Y, Direction);
                map.MoveBombAction(this);
            }
        }

        #endregion
    }

    public class BonusEntity : Entity, ITimedEntity
    {
        public DateTime FadeoutRemainingTimeout { get; set; } // in ms

        public BonusEntity(EntityTypes type, int x, int y, int delayInMs)
            : base(type, x, y)
        {
            FadeoutRemainingTimeout = DateTime.Now.AddMilliseconds(delayInMs);
        }

        #region ITimedEntity

        public bool IsTimeoutElapsed(DateTime now)
        {
            return FadeoutRemainingTimeout <= now;
        }
        
        public void TimeoutAction(DateTime now, CellMap map)
        {
            Log.WriteLine(Log.LogLevels.Debug, "Bonus fades out at {0},{1}", X, Y);
            // Remove bonus from map
            map.RemoveEntity(this);
            // Remove bonus in every player map
            foreach (IPlayer player in map.PlayerManager.Players)
                player.OnEntityDeleted(Type, X, Y);
        }

        #endregion
    }

    public class FlameEntity : Entity, ITimedEntity
    {
        public DateTime FadeoutRemainingTimeout { get; set; } // in ms

        public FlameEntity(int x, int y, int delayInMs)
            : base(EntityTypes.Flames, x, y)
        {
            FadeoutRemainingTimeout = DateTime.Now.AddMilliseconds(delayInMs);
        }

        #region ITimedEntity

        public bool IsTimeoutElapsed(DateTime now)
        {
            return FadeoutRemainingTimeout <= now;
        }

        public void TimeoutAction(DateTime now, CellMap map)
        {
            Log.WriteLine(Log.LogLevels.Debug, "Flame fades out at {0},{1}", X, Y);
            // Remove flame from map
            map.RemoveEntity(this);
            // Remove flame in every player map
            foreach (IPlayer player in map.PlayerManager.Players)
                player.OnEntityDeleted(Type, X, Y);
        }

        #endregion
    }

    public class Cell : List<Entity>
    {
        public Entity GetEntity(EntityTypes type)
        {
            return this.FirstOrDefault(x => x.Type == type);
        }
    }

    public interface IServerMapInteraction
    {
        bool IsGameStarted { get; }

        void MoveBomb(BombEntity bomb);

        void OnExplosion(int x, int y, int range); // Handle explosion
        void OnEntityAddedInMap(EntityTypes entity, int x, int y); // Warn players about entity added
        void OnEntityDeletedInMap(EntityTypes entity, int x, int y); // Warn players about entity deleted
        void OnEntityMovedInMap(EntityTypes entity, int fromX, int fromY, int toX, int toY);
    }

    public class CellMap
    {
        private readonly IServerMapInteraction _serverMapInteraction;
        private readonly object _timedEntityCollectionLock = new object();
        private readonly List<ITimedEntity> _timedEntityCollection; // TODO: PriorityQueue allowing to remove elements anywhere
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _timedEntityTask;

        private Cell[,] _cells;

        public Map OriginalMap { get; private set; }
        public int Size { get; private set; }
        public IPlayerManager PlayerManager { get; private set; }

        public CellMap(IServerMapInteraction serverMapInteraction, IPlayerManager playerManager)
        {
            _serverMapInteraction = serverMapInteraction;
            PlayerManager = playerManager;
            _timedEntityCollection = new List<ITimedEntity>();

            _cancellationTokenSource = new CancellationTokenSource();
            _timedEntityTask = new Task(TimedEntityTask, _cancellationTokenSource.Token);
        }

        public void Initialize(Map map)
        {
            // Clear timed entity
            lock (_timedEntityCollectionLock)
                _timedEntityCollection.Clear();
            // Create cells from map
            OriginalMap = map;
            Size = map.Description.Size;
            _cells = new Cell[Size, Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    _cells[x, y] = new Cell
                    {
                        new Entity(map.GetEntity(x,y), x, y)
                    };
            _timedEntityTask.Start();
        }

        public void Clear()
        {
            // Clear list and timed entity
            lock(_timedEntityCollectionLock)
                _timedEntityCollection.Clear();
            if (_cells != null)
                foreach(Cell cell in _cells)
                    cell.Clear();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _timedEntityTask.Wait(1000);
        }

        public Cell GetCell(int x, int y)
        {
            return _cells[x, y];
        }
        
        public Entity GetEntity(EntityTypes type, int x, int y)
        {
            return _cells[x, y].GetEntity(type);
        }

        public void AddEntity(Entity entity)
        {
            Cell cell = _cells[entity.X, entity.Y];
            cell.Add(entity);
            if (entity is ITimedEntity)
                AddTimeoutEntity(entity as ITimedEntity);

            _serverMapInteraction.OnEntityAddedInMap(entity.Type, entity.X, entity.Y);
        }

        public void RemoveEntity(Entity entity)
        {
            Cell cell = _cells[entity.X, entity.Y];
            bool removed = cell.Remove(entity);
            // Remove from timeout entities
            if (entity is ITimedEntity)
            {
                lock (_timedEntityCollectionLock)
                    _timedEntityCollection.RemoveAll(x => x == entity);
            }

            if (removed)
                _serverMapInteraction.OnEntityDeletedInMap(entity.Type, entity.X, entity.Y);
        }

        public void MoveEntity(Entity entity, int toX, int toY)
        {
            int fromX = entity.X;
            int fromY = entity.Y;
            Cell fromCell = _cells[fromX, fromY];
            bool entityFound = fromCell.Any(x => x == entity);
            if (entityFound)
            {
                Cell toCell = _cells[toX, toY];
                fromCell.Remove(entity);
                toCell.Add(entity);
                entity.X = toX;
                entity.Y = toY;

                _serverMapInteraction.OnEntityMovedInMap(entity.Type, fromX, fromY, toX, toY);
            }
            else
                Log.WriteLine(Log.LogLevels.Error, "Cell at {0},{1} doesn't contain {2}", entity.X, entity.Y, entity.Type);
        }

        public void AddTimeoutEntity(ITimedEntity entity)
        {
            lock(_timedEntityCollectionLock)
                _timedEntityCollection.Add(entity);
        }

        public void RemoveTimeoutEntity(ITimedEntity entity)
        {
            lock (_timedEntityCollectionLock)
                _timedEntityCollection.Remove(entity);
        }

        public void ExplosionAction(int x, int y, int range)
        {
            _serverMapInteraction.OnExplosion(x, y, range);
        }

        public void MoveBombAction(BombEntity bomb)
        {
            _serverMapInteraction.MoveBomb(bomb);
        }

        // Tasks
        private void TimedEntityTask()
        {
            const int sleepTime = 25;
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    break;

                // Check if map timer is elapsed
                if (_serverMapInteraction.IsGameStarted)
                {
                    // Get timed out entities
                    List<ITimedEntity> timedOutEntities;
                    lock(_timedEntityCollectionLock)
                        timedOutEntities = _timedEntityCollection.Where(x => x.IsTimeoutElapsed(DateTime.Now)).ToList();
                    if (timedOutEntities.Any())
                    {
                        // Remove timed out entities from list
                        lock (_timedEntityCollectionLock)
                            _timedEntityCollection.RemoveAll(timedOutEntities.Contains);
                        // Call timeout action
                        foreach (ITimedEntity timedEntity in timedOutEntities)
                            timedEntity.TimeoutAction(DateTime.Now, this);
                    }
                    bool signaled = _cancellationTokenSource.Token.WaitHandle.WaitOne(sleepTime);
                    if (signaled)
                        break;
                }
            }
        }

        // Helpers
        public void ComputeNewCoordinates(Entity entity, Directions direction, out int newX, out int newY)
        {
            int stepX, stepY;
            GetDirectionSteps(direction, out stepX, out stepY);
            newX = ComputeLocation(entity.X, stepX);
            newY = ComputeLocation(entity.Y, stepY);
        }

        private int ComputeLocation(int coord, int step)
        {
            coord = (coord + step) % Size;
            if (coord < 0)
                coord += Size;
            return coord;
        }

        public static void GetDirectionSteps(Directions direction, out int stepX, out int stepY)
        {
            stepX = 0;
            stepY = 0;
            switch (direction)
            {
                case Directions.Left:
                    stepX = -1;
                    break;
                case Directions.Right:
                    stepX = +1;
                    break;
                case Directions.Up:
                    stepY = -1;
                    break;
                case Directions.Down:
                    stepY = +1;
                    break;
            }
        }
    }
}
