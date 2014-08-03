﻿using System;
using System.Collections.Generic;
using System.Linq;
using ClientWPF.ViewModels;
using Common.DataContract;

namespace ClientWPF.Logic
{
    public class ClientProcessor
    {
        public Player Player { get; set; }

        public Map Map { get; set; }

        public static BombermanViewModel BombermanViewModel { get; set; }

        public void OnUserConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            BombermanViewModel.OnUserConnected(player, loginsList, canStartGame);
        }

        public void OnGameStarted(Game newGame)
        {
            InitializeConsole();
            DisplayMap(newGame);
        }

        private static void InitializeConsole()
        {
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            if (Map == null) return;
            //check if object to move does exists
            if (!Map.GridPositions.Any(livingObject => livingObject.ComparePosition(objectToMoveBefore))) return;
            //if before is player and is "me" then update global player
            if (objectToMoveBefore is Player && Player.CompareId(objectToMoveBefore))
                Player = objectToMoveAfter as Player;
            //handle before
            Console.SetCursorPosition(objectToMoveBefore.ObjectPosition.PositionX, 10 + objectToMoveBefore.ObjectPosition.PositionY); // 10 should be replaced with map parameters
            Console.Write(' ');
            Map.GridPositions.Remove(objectToMoveBefore);
            //handle after
            char toDisplay = ObjectToChar(objectToMoveAfter);
            Console.SetCursorPosition(objectToMoveAfter.ObjectPosition.PositionX, 10 + objectToMoveAfter.ObjectPosition.PositionY); // 10 should be replaced with map parameters
            Console.Write(toDisplay);
            Map.GridPositions.Add(objectToMoveAfter);
        }

        private void DisplayMap(Game currentGame)
        {
            Map = currentGame.Map;
            foreach (LivingObject item in currentGame.Map.GridPositions)
            {
                Console.SetCursorPosition(item.ObjectPosition.PositionX, 10 + item.ObjectPosition.PositionY); // 10 should be replaced with map parameters
                char toDisplay = ObjectToChar(item);
                
                Console.Write(toDisplay);
            }
        }

        private char ObjectToChar(LivingObject item)
        {
            if (item is Wall)
            {
                var wall = item as Wall;
                return wall.WallType == WallType.Undestructible ? '█' : '.';
            }
            if (item is Player)
            {
                var player = item as Player;
                return Player.CompareId(player) ? 'X' : '*';
            }
            return ' ';
        }
    }
}
