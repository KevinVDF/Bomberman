using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataContract;

namespace Client.Logic
{
    public class ClientProcessor
    {
        public Player Player { get; set; }

        public Map Map { get; set; }

        public void OnError(string errorMessage, ErrorType errorType)
        {
            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("----------- ERROR --------\n\n");
            Console.WriteLine(errorMessage);
        }

        public void OnConnection(Player player, IEnumerable<String> loginsList)
        {
            Player = player;

            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("-----------" + Player.Username + "--------\n\n");

            Console.WriteLine("List of players online :\n\n");
            foreach (string login in loginsList)
            {
                Console.WriteLine(login + "\n\n");
            }
            Console.WriteLine("Press S to start the game");
        }

        public void OnUserConnected(IEnumerable<String> loginsList)
        {
            
             InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("-----------" + Player.Username + "--------\n\n");

            Console.WriteLine("A new player  joined the room\n\n");
            Console.WriteLine("List of players online :\n\n");
            foreach (string login in loginsList)
            {
                Console.WriteLine(login + "\n\n");
            }
            Console.WriteLine("Press S to start the game");
        }

        public void OnUserDisconnected(IEnumerable<String> loginsList)
        {
            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("-----------" + Player.Username + "--------\n\n");

            Console.WriteLine("A player leaved the room\n\n");
            Console.WriteLine("List of players online :\n\n");
            foreach (string login in loginsList)
            {
                Console.WriteLine(login + "\n\n");
            }
            Console.WriteLine("Press S to start the game");
        }

        public void OnGameStarted(Game newGame)
        {
            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("---------------FIGHT!-----------------");
            Console.WriteLine("--------------------------------------");
            DisplayMap(newGame);
        }

        private static void InitializeConsole()
        {
            Console.SetWindowSize(80, 30);
            Console.BufferWidth = 80;
            Console.BufferHeight = 30;
            Console.CursorVisible = false;
            Console.Clear();
        }

        public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        {
            if (Map == null) return;
            //check if object to move does exists
            if (!Map.LivingObjects.Any(livingObject => livingObject.ComparePosition(objectToMoveBefore))) return;
            //if before is player and is "me" then update global player
            if (objectToMoveBefore is Player && Player.ID == objectToMoveBefore.ID)
                Player = objectToMoveAfter as Player;
            //handle before
            Console.SetCursorPosition(objectToMoveBefore.Position.X, 10 + objectToMoveBefore.Position.Y); // 10 should be replaced with map parameters
            Console.Write(' ');
            Map.LivingObjects.Remove(objectToMoveBefore);
            //handle after
            char toDisplay = ObjectToChar(objectToMoveAfter);
            Console.SetCursorPosition(objectToMoveAfter.Position.X, 10 + objectToMoveAfter.Position.Y); // 10 should be replaced with map parameters
            Console.Write(toDisplay);
            Map.LivingObjects.Add(objectToMoveAfter);
        }

        private void DisplayMap(Game currentGame)
        {
            Map = currentGame.Map;
            foreach (LivingObject item in currentGame.Map.LivingObjects)
            {
                Console.SetCursorPosition(item.Position.X, 10 + item.Position.Y); // 10 should be replaced with map parameters
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
                return Player.ID == player.ID ? 'X' : '*';
            }
            return ' ';
        }
    }
}
