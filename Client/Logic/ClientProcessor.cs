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

        public void OnConnected(Player player, List<String> loginsList, bool canStartGame)
        {
            Player = player;

            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("--------------------------------------\n\n");
            Console.WriteLine("New User Joined the server : " + player.Username);
            Console.WriteLine("List of players online :");
            foreach (string login in loginsList)
                Console.WriteLine(login);
            if (Player.IsCreator)
                Console.WriteLine(canStartGame ? "Press S to start the game" : "Wait for other players.");
            else Console.WriteLine("Wait until the creator start the game.");
        }

        public void OnUserConnected(List<String> loginsList)
        {
            InitializeConsole();
            Console.WriteLine("New User Joined the server");
            Console.WriteLine("List of players online :");
            foreach (string login in loginsList)
                Console.WriteLine(login);
        }

        public void OnGameStarted(Game newGame)
        {
            Map = newGame.Map;
            InitializeConsole();
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("-------- Welcome to Bomberman --------");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("---------------FIGHT!-----------------");
            Console.WriteLine("--------------------------------------");
            DisplayMap();
        }

        private static void InitializeConsole()
        {
            Console.SetWindowSize(80, 30);
            Console.BufferWidth = 80;
            Console.BufferHeight = 30;
            Console.CursorVisible = false;
            Console.Clear();
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            if (Map == null)
                return;
            LivingObject playerInMap = Map.GridPositions.First(x => x.ID == player.ID);
            if (playerInMap != null)
                playerInMap.Position = newPosition;
            DisplayMap();
        }

        public void OnBombDropped(Bomb bomb)
        {
            if (Map == null)
                return;
            Map.GridPositions.Add(bomb);
            DisplayMap();
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            if (Map == null)
                return;
            foreach (var livingObject in impacted)
            {
                Map.GridPositions.Remove(livingObject);
            }
            DisplayMap();
        }

        public void OnPlayerDeath(Player player)
        {
            if (Map == null)
                return;
            Map.GridPositions.Remove(player);
            DisplayMap();
        }

        public void OnMyDeath()
        {
            if (Map == null)
                return;
            Map.GridPositions.Remove(Player);
            DisplayMap();
        }

        //public void OnMove(LivingObject objectToMoveBefore, LivingObject objectToMoveAfter)
        //{
        //    if (Map == null) 
        //        return;
        //    //check if object to move does exists
        //    if (!Map.GridPositions.Any(livingObject => livingObject.ComparePosition(objectToMoveBefore))) 
        //        return;
        //    //if before is player and is "me" then update global player
        //    if (objectToMoveBefore is Player && Player.CompareId(objectToMoveBefore))
        //        Player = objectToMoveAfter as Player;
        //    //handle before
        //    Console.SetCursorPosition(objectToMoveBefore.Position.X, 10 + objectToMoveBefore.Position.Y); // 10 should be replaced with map parameters
        //    Console.Write(' ');
        //    Map.GridPositions.Remove(objectToMoveBefore);
        //    //handle after
        //    char toDisplay = ObjectToChar(objectToMoveAfter);
        //    Console.SetCursorPosition(objectToMoveAfter.Position.X, 10 + objectToMoveAfter.Position.Y); // 10 should be replaced with map parameters
        //    Console.Write(toDisplay);
        //    Map.GridPositions.Add(objectToMoveAfter);
        //}

        public void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        private void DisplayMap()
        {
            Console.Clear();
            foreach (LivingObject item in Map.GridPositions)
            {
                Console.SetCursorPosition(item.Position.X, 10 + item.Position.Y); // TODO: replace 10
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
                return Player.ID == player.ID ? 'X' : 'E';
            }
            if (item is Bomb)
            {
                return '*';
            }
            return ' ';
        }
    }
}
