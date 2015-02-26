﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Common.DataContract;
using Common.Log;
using Server.Manager.Interface;

namespace Server.Manager
{
    public class MapManager: IMapManager
    {
        private IUserManager _userManager;

        public MapManager(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Map GenerateMap(string mapName)
        {
            int mapSize;
            Map map = new Map();
            List<LivingObject> matrice = new List<LivingObject>();

            IEnumerable<User> users = _userManager.GetAllUsers();

            if (users == null || !users.Any())
            {
                Log.WriteLine(Log.LogLevels.Error, "Problem getting users");
                return null;
            }

            User[] players = users.ToArray();

            int idCount = players.Count();
            using (StreamReader reader = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["MapPath"], mapName), Encoding.UTF8))
            {
                // Read size
                string size = reader.ReadLine();
                bool isSizeValid = int.TryParse(size, out mapSize);

                if (!isSizeValid)
                {
                    Log.WriteLine(Log.LogLevels.Error, "Invalid map size {0}", size);
                    return null;
                }
                // Read map
                string objectsToAdd = reader.ReadToEnd().Replace("\n", "").Replace("\r", "");

                for (int y = 0; y < mapSize; y++)
                {
                    for (int x = 0; x < mapSize; x++)
                    {
                        LivingObject livingObject = null;
                        char cell = objectsToAdd[(y * mapSize) + x];
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
                                    ,
                                    ID = idCount++
                                };
                                Log.WriteLine(Log.LogLevels.Debug, "New undestructible wall created");
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
                                    ,
                                    ID = idCount++
                                };
                                Log.WriteLine(Log.LogLevels.Debug, "New destructible wall created");
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
                                if (players.Count() > (int)Char.GetNumericValue(cell))
                                {
                                    livingObject = players[(int) Char.GetNumericValue(cell)].Player = new Player
                                    {
                                        Alive = true,
                                        ID = (int)Char.GetNumericValue(cell),
                                        BombNumber = 1,
                                        BombPower = 1,
                                        CanShootBomb = false,
                                        Score = 0
                                    };
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

            if (matrice.Count == 0)
            {
                Log.WriteLine(Log.LogLevels.Error, "Probleme creating objects");
                return null;
            }
            map.LivingObjects = matrice;
            map.MapName = mapName;
            map.MapSize = mapSize;

            return map;
        }
    }
}
