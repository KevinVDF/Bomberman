using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientWPF.Model;
using ClientWPF.ViewModels.StartedGame;
using Common.DataContract;
using Common.Log;

namespace ClientWPF.Textures
{
    public static class Textures
    {

        #region Properties  

        public const int NumberImagesPerPlayer = 12;

        public const int PlayerStartImageY = 38;

        public const int PlayerStartImageX = 52;

        public const int PlayerWidth = 17;

        public const int PlayerHeight = 24;

        public const int DownImagePosition = 1;

        public const int LeftImagePosition = 4;

        public const int RightImagePosition = 7;

        public const int UpImagePosition = 10;

        public const int StartedImage = 2;

        public const int SpaceBetweenPlayer = 4;

        public const int SpaceBetweenImages = 1;

        public static string GlobalImagePath = ConfigurationManager.AppSettings["ImagePath"];

        public static string GlobalImageName = "13Bomberman.png";

        public static PlayerItem Player1Item;

        public static PlayerItem Player2Item;

        public static PlayerItem Player3Item;

        public static PlayerItem Player4Item;

        public static WallItem DestructibleWallItem;

        public static WallItem UndestructibleWallItem;

        public static BombItem BombItem;

        public static BombItem ExplodedBombItem;

        #endregion Properties

        public static void InitializeItem()
        {
            Player1Item = new PlayerItem();
            Player2Item = new PlayerItem();
            Player3Item = new PlayerItem();
            Player4Item = new PlayerItem();
            DestructibleWallItem = new WallItem
            {
                WallType = WallType.Destructible
            };
            UndestructibleWallItem = new WallItem
            {
                WallType = WallType.Undestructible
            };
            BombItem = new BombItem();
            ExplodedBombItem = new BombItem();

            GetSpriteForPlayer(Player1Item, 0);
            GetSpriteForPlayer(Player2Item, 1);
            GetSpriteForPlayer(Player3Item, 2);
            GetSpriteForPlayer(Player4Item, 3);
            GetSpriteForWall(DestructibleWallItem);
            GetSpriteForWall(UndestructibleWallItem);
            GetSpriteForBomb(BombItem);
            GetSpriteForExplodedBomb(ExplodedBombItem);
        }

        private static void GetSpriteForPlayer(PlayerItem player, int playerNumber)
        {
            string imagePath = String.Format(@"{0}\{1}", GlobalImagePath, GlobalImageName);
            BitmapImage imageBmp = new BitmapImage(new Uri(imagePath));
            //initialize textures
            player.Textures = new Sprite();
            for (int i = 0; i < NumberImagesPerPlayer; i++)
            {
                Extract(player, playerNumber, imageBmp, i);
            }
        }

        private static void GetSpriteForWall(WallItem wall)
        {
            switch (wall.WallType)
            {
                case WallType.Undestructible:
                    wall.ImageInUse = new ImageBrush(new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Undestructible.png")));
                    break;
                case WallType.Destructible:
                    wall.ImageInUse = new ImageBrush(new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Destructible.png")));
                    break;
            }
        }

        private static void GetSpriteForBomb(BombItem bomb)
        {
            string imagePath = String.Format(@"{0}\{1}", GlobalImagePath, GlobalImageName);
            bomb.ImageInUse = ExtractBackground(new BitmapImage(new Uri(imagePath)), 20, 258, 12, 14);
        }

        private static void GetSpriteForExplodedBomb(BombItem bomb)
        {
            string imagePath = String.Format(@"{0}\{1}", GlobalImagePath, GlobalImageName);
            bomb.ImageInUse = ExtractBackground(new BitmapImage(new Uri(imagePath)), 154, 259, 78, 78);
        }

        private static void Extract(PlayerItem player, int playerNumber, BitmapImage imageBmp, int imageNumber)
        {   
            int posX = PlayerStartImageX + imageNumber*(PlayerWidth+SpaceBetweenImages);
            int posY = PlayerStartImageY + playerNumber*(PlayerHeight + SpaceBetweenPlayer);

            Brush brush = ExtractBackground(imageBmp, posX, posY, PlayerWidth, PlayerHeight);

            switch (imageNumber)
            {
                case 0:
                case 1:
                case 2:
                    player.Textures.Down.Add(brush);
                    break;
                case 3: 
                case 4:
                case 5:
                    player.Textures.Left.Add(brush);
                    break;
                case 6: 
                case 7:
                case 8:
                    player.Textures.Right.Add(brush);
                    break;
                default: 
                    player.Textures.Up.Add(brush);
                    break;
            }

            if (imageNumber == StartedImage)
                player.ImageInUse = player.Textures.Down[1];//every player will start with face down

        }

        private static Brush ExtractBackground(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.Fill
                };
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Error, "Error while extracting background texture. Image {0}. Exception: {1}", image.BaseUri, ex);
            }
            return background;
        }
    }
}
