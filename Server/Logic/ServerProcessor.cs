﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Common.DataContract;
using Common.Interfaces;
using Server.Model;

namespace Server.Logic
{
    public class ServerProcessor
    {
        #region variables

        private static int MapSize = 10; // SinaC: why static ?

        private static ServerModel Server; // SinaC: why static ?

        #endregion variables

        #region methods

        public void StartServer()
        {
            Server = new ServerModel();
            Server.Initialize();
        }

        public void ConnectUser(string login)
        {
            //create new Player
            PlayerModel newPlayer = new PlayerModel
                {
                    CallbackService = OperationContext.Current.GetCallbackChannel<IBombermanCallbackService>(),
                    Login = login,
                    IsCreator = Server.PlayersOnline.Count == 0
                };
            //register user to the server
            Server.PlayersOnline.Add(newPlayer);
            //create a list of login to send to client
            List<string> logins = Server.PlayersOnline.Select(x => x.Login).ToList();
            //Warning players that a new player is connected and send them the list of all players online
            bool canStartGame = Server.PlayersOnline.Count > 1; // SinaC: no need to compute this in inner loop
            foreach (PlayerModel currentPlayer in Server.PlayersOnline)
            {
                //todo check if player disconnect
                currentPlayer.CallbackService.OnUserConnected(login, logins, currentPlayer.IsCreator, canStartGame);
            }
        }

        public void StartGame()
        {
            //create the list of players to pass to client
            List<Player> players = Server.PlayersOnline.Select(playerModel => new Player
                {
                    Username = playerModel.Login
                }).ToList();

            Game newGame = new Game
                {
                    Map = new Map
                        {
                            MapName = "Custom Map",
                            GridPositions = GenerateGrid(players)
                        },
                    CurrentStatus = GameStatus.Started,
                };
            Server.GameCreated = newGame;
            foreach (PlayerModel currentPlayer in Server.PlayersOnline)
            {
                currentPlayer.CallbackService.OnGameStarted(newGame, currentPlayer.Login);
            }
        }

        public void MovePlayerToLocation(string login, ActionType actionType)
        {
            Player currentPlayer = null; // no need to use a LivingObject if we cast it to Player
            //find the player to move and initialize the current position
            foreach (var item in Server.GameCreated.Map.GridPositions.Where(x => x is Player))
            {
                currentPlayer = item as Player;
                if (currentPlayer != null && currentPlayer.Username == login)
                    break;
            }
            // SinaC: previous loop should be replaced with this Linq query
            //  previous loop can lead to false result if login is not found in GridPositions, currentPlayer would be equal to last player in GridPositions
            //Player currentPlayer = Server.GameCreated.Map.GridPositions.Where(x => x is Player).Cast<Player>().FirstOrDefault(x => x.Username == login);
            switch (actionType)
            {
                case ActionType.MoveUp:
                    //MoveUp(currentPlayer);
                    Move(currentPlayer, 0, -1);
                    break;
                case ActionType.MoveDown:
                    //MoveDown(currentPlayer);
                    Move(currentPlayer, 0, +1);
                    break;
                case ActionType.MoveRight:
                    //MoveRight(currentPlayer);
                    Move(currentPlayer, +1, 0);
                    break;
                case ActionType.MoveLeft:
                    //MoveLeft(currentPlayer);
                    Move(currentPlayer, -1, 0);
                    break;
            }
        }

        private List<LivingObject> GenerateGrid(List<Player> players) // SinaC: path to map should be an additional parameter
        {
            List<LivingObject> matrice = new List<LivingObject>();

            using (StreamReader reader = new StreamReader(@"D:\github\Bomberman\Server\Map.dat", Encoding.UTF8))
            {
                string objectsToAdd = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");

                for (int y = 0; y < MapSize; y++)
                {
                    for (int x = 0; x < MapSize; x++)
                    {
                        LivingObject currentlivingObject = null;
                        char cell = objectsToAdd[(y*MapSize) + x]; // SinaC: factoring is the key :)   y and x were inverted
                        switch (cell)
                        {
                            case 'u':
                                currentlivingObject = new Wall
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
                                currentlivingObject = new Wall
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
                                if (players.Count > (int) Char.GetNumericValue(cell))
                                {
                                    currentlivingObject = new Player
                                        {
                                            Username = players[(int) Char.GetNumericValue(cell)].Username,
                                            ObjectPosition = new Position
                                                {
                                                    PositionX = x,
                                                    PositionY = y
                                                }
                                        };
                                }
                                break;
                        }
                        if (currentlivingObject != null)
                            matrice.Add(currentlivingObject);
                    }
                }
            }
            return matrice;
        }


        // SinaC: replace MoveUp/Down/Left and Right   use Move(0,-1) for MoveUp, Move(0,+1) for MoveDown, ...
        private void Move(Player before, int stepX, int stepY)
        {
            // Get object at future player location
            LivingObject collider = Server.GameCreated.Map.GridPositions.FirstOrDefault(x => before.ObjectPosition.PositionY + stepY == x.ObjectPosition.PositionY
                                                                                             && before.ObjectPosition.PositionX + stepX == x.ObjectPosition.PositionX);
            // Can't go thru wall
            if (collider is Wall)
                return;

            // Update position by creating a new player
            Player after = new Player
            {
                Username = before.Username,
                ObjectPosition = new Position
                {
                    PositionX = before.ObjectPosition.PositionX + stepX,
                    PositionY = before.ObjectPosition.PositionY + stepY,
                }
            };

            // Remove player from old position
            Server.GameCreated.Map.GridPositions.Remove(before);
            // And add to new position
            Server.GameCreated.Map.GridPositions.Add(after);

            // Send new player position to players
            foreach (PlayerModel playerModel in Server.PlayersOnline)
                playerModel.CallbackService.OnMove(before, after);
        }

        /*
        private void MoveUp(Player currentPlayerBefore)
        {
            LivingObject currentPlayerAfter = new Player
                {
                    ObjectPosition = new Position
                        {
                            PositionX = currentPlayerBefore.ObjectPosition.PositionX,
                            PositionY = currentPlayerBefore.ObjectPosition.PositionY
                        },
                    Username = currentPlayerBefore.Username
                };
            //retreive object positionned just above the current player position
            LivingObject objectToNextPosition = Server.GameCreated.Map.GridPositions.FirstOrDefault(x => x.ObjectPosition.PositionY == currentPlayerBefore.ObjectPosition.PositionY - 1
                                                                                                         && x.ObjectPosition.PositionX == currentPlayerBefore.ObjectPosition.PositionX);
            //if its a Wall then return
            if (objectToNextPosition is Wall) return;
            //change position
            currentPlayerAfter.ObjectPosition.PositionY = currentPlayerBefore.ObjectPosition.PositionY - 1;
            //modify the currentMap
            Server.GameCreated.Map.GridPositions.Remove(currentPlayerBefore);
            Server.GameCreated.Map.GridPositions.Add(currentPlayerAfter);
            //warn each player of the move
            foreach (PlayerModel playerModel in Server.PlayersOnline)
            {
                playerModel.CallbackService.OnMove(currentPlayerBefore, currentPlayerAfter);
            }
        }

        private void MoveDown(Player currentPlayerBefore)
        {
            LivingObject currentPlayerAfter = new Player
                {
                    ObjectPosition = new Position
                        {
                            PositionX = currentPlayerBefore.ObjectPosition.PositionX,
                            PositionY = currentPlayerBefore.ObjectPosition.PositionY
                        },
                    Username = currentPlayerBefore.Username
                };
            //retreive object positionned just above the current player position
            LivingObject objectToNextPosition = Server.GameCreated.Map.GridPositions.FirstOrDefault(x => x.ObjectPosition.PositionY == currentPlayerBefore.ObjectPosition.PositionY + 1
                                                                                                         && x.ObjectPosition.PositionX == currentPlayerBefore.ObjectPosition.PositionX);
            //if its a Wall then return
            if (objectToNextPosition is Wall) return;
            //change position
            currentPlayerAfter.ObjectPosition.PositionY = currentPlayerBefore.ObjectPosition.PositionY + 1;
            //modify the currentMap
            Server.GameCreated.Map.GridPositions.Remove(currentPlayerBefore);
            Server.GameCreated.Map.GridPositions.Add(currentPlayerAfter);
            //warn each player of the move
            foreach (PlayerModel playerModel in Server.PlayersOnline)
            {
                playerModel.CallbackService.OnMove(currentPlayerBefore, currentPlayerAfter);
            }
        }

        private void MoveRight(Player currentPlayerBefore)
        {
            LivingObject currentPlayerAfter = new Player
                {
                    ObjectPosition = new Position
                        {
                            PositionX = currentPlayerBefore.ObjectPosition.PositionX,
                            PositionY = currentPlayerBefore.ObjectPosition.PositionY
                        },
                    Username = currentPlayerBefore.Username
                };
            //retreive object positionned just above the current player position
            LivingObject objectToNextPosition = Server.GameCreated.Map.GridPositions.FirstOrDefault(x => x.ObjectPosition.PositionY == currentPlayerBefore.ObjectPosition.PositionY
                                                                                                         && x.ObjectPosition.PositionX == currentPlayerBefore.ObjectPosition.PositionX + 1);
            //if its a Wall then return
            if (objectToNextPosition is Wall) return;
            //change position
            currentPlayerAfter.ObjectPosition.PositionX = currentPlayerBefore.ObjectPosition.PositionX + 1;
            //modify the currentMap
            Server.GameCreated.Map.GridPositions.Remove(currentPlayerBefore);
            Server.GameCreated.Map.GridPositions.Add(currentPlayerAfter);
            //warn each player of the move
            foreach (PlayerModel playerModel in Server.PlayersOnline)
            {
                playerModel.CallbackService.OnMove(currentPlayerBefore, currentPlayerAfter);
            }
        }

        private void MoveLeft(Player currentPlayerBefore)
        {
            LivingObject currentPlayerAfter = new Player
                {
                    ObjectPosition = new Position
                        {
                            PositionX = currentPlayerBefore.ObjectPosition.PositionX,
                            PositionY = currentPlayerBefore.ObjectPosition.PositionY
                        },
                    Username = currentPlayerBefore.Username
                };
            //retreive object positionned just above the current player position
            LivingObject objectToNextPosition = Server.GameCreated.Map.GridPositions.FirstOrDefault(x => x.ObjectPosition.PositionY == currentPlayerBefore.ObjectPosition.PositionY
                                                                                                         && x.ObjectPosition.PositionX == currentPlayerBefore.ObjectPosition.PositionX - 1);
            //if its a Wall then return
            if (objectToNextPosition is Wall) return;
            //change position
            currentPlayerAfter.ObjectPosition.PositionX = currentPlayerBefore.ObjectPosition.PositionX - 1;
            //modify the currentMap
            Server.GameCreated.Map.GridPositions.Remove(currentPlayerBefore);
            Server.GameCreated.Map.GridPositions.Add(currentPlayerAfter);
            //warn each player of the move
            foreach (PlayerModel playerModel in Server.PlayersOnline)
            {
                playerModel.CallbackService.OnMove(currentPlayerBefore, currentPlayerAfter);
            }
        }
        */
        #endregion methods
    }
}
