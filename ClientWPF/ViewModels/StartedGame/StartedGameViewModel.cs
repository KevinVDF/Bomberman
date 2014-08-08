using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientWPF.Textures;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;

namespace ClientWPF.ViewModels.StartedGame
{
    public class StartedGameViewModel : ViewModelBase
    {
        #region Properties

        public string GlobalImagePath { get; set; }

        public const int MiddleImage = 2;

        public static IBombermanService Proxy;

        private MapViewModel _mapViewModel;
        public MapViewModel MapViewModel
        {
            get { return _mapViewModel; }
            set { Set(() => MapViewModel, ref _mapViewModel, value); }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(() => IsVisible, ref _isVisible, value); }
        }

        #endregion 

        #region Methods

        //Constructor
        public StartedGameViewModel()
        {
            MapViewModel = new MapViewModel();
        }

        public void Initialize(bool isVisible, IBombermanService proxy)
        {
            Proxy = proxy;
            IsVisible = isVisible;
            MapViewModel.Initialize(proxy);
            GlobalImagePath = ConfigurationManager.AppSettings["ImagePath"];
        }

        public void InitializePlayer(Common.DataContract.Player player)
        {
            MapViewModel.InitializePlayer(player);
        }

        public void RegisterGame(Game newGame)
        {
            int playerNumber = 0;
            ObservableCollection<LivingObjectItem> livingObjectItems = new ObservableCollection<LivingObjectItem>();

            foreach (LivingObject livingObject in newGame.Map.GridPositions)
            {
                LivingObjectItem livingObjectItem = null;
                if (livingObject is Wall)
                {
                    livingObjectItem = new WallItem
                    {
                        PositionX = livingObject.ObjectPosition.PositionX,
                        PositionY = livingObject.ObjectPosition.PositionY,
                        WallType = ((Wall) livingObject).WallType
                    };
                    GetSpriteForWall(livingObjectItem);
                }
                if (livingObject is Player)
                {
                    livingObjectItem = new PlayerItem
                    {
                        PositionX = livingObject.ObjectPosition.PositionX,
                        PositionY = livingObject.ObjectPosition.PositionY,
                    };
                    
                    GetSpriteForPlayer(livingObjectItem as PlayerItem, playerNumber);
                    playerNumber++;
                }
                
                livingObjectItems.Add(livingObjectItem);
            }
            MapViewModel.LivingObjects = livingObjectItems;
        }

        private void GetSpriteForPlayer(PlayerItem player, int playerNumber)
        {
            string playersImagePath = String.Format(@"{0}\13Bomberman.png", GlobalImagePath);
            BitmapImage imageBmp = new BitmapImage(new Uri(playersImagePath));

            for (int i = 0; i < TexturesPosition.NumberImagesPerSpriteByPlayer; i++)
            {
                //down
                ExtractDown(player, playerNumber, imageBmp, i, TexturesPosition.DownImagePosition);
                ////left
                //ExtractLeft(playerItem, playerNumber, playersImagePath);
                ////right
                //ExtractRight(playerItem, playerNumber, playersImagePath);
                ////up
                //ExtractUp(playerItem, playerNumber, playersImagePath);
            }
        }

        private void GetSpriteForWall(LivingObjectItem livingObjectItem)
        {
            switch (((WallItem)livingObjectItem).WallType)
            {
                case WallType.Undestructible:
                    livingObjectItem.ImageInUse = new ImageBrush(new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Undestructible.png")));
                    break;
                case WallType.Destructible:
                    livingObjectItem.ImageInUse = new ImageBrush(new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Destructible.png")));
                    break;
            }
        }

        //private void ExtractUp(PlayerItem playerItem, int playerNumber, string playersImagePath)
        //{
        //    switch (playerNumber)
        //    {
        //        case 1:
        //            playerItem.Down.Images.Add(ExtractBackground(new BitmapImage(
        //                new Uri(playersImagePath)), TexturesPosition.Player1StartImageX, TexturesPosition.Player1StartImageY, TexturesPosition.PlayerWidth, TexturesPosition.PlayerHeight));
        //            break;
        //    }
        //}

        //private void ExtractRight(PlayerItem playerItem, int playerNumber, string playersImagePath)
        //{
        //    switch (playerNumber)
        //    {
        //        case 1:
        //            playerItem.Down.Images.Add(ExtractBackground(new BitmapImage(
        //                new Uri(playersImagePath)), TexturesPosition.PlayerStartImageX, TexturesPosition.PlayerStartImageY, TexturesPosition.PlayerWidth, TexturesPosition.PlayerHeight));
        //            break;
        //    }
        //}

        //private void ExtractLeft(PlayerItem playerItem, int playerNumber, string playersImagePath)
        //{
        //    switch (playerNumber)
        //    {
        //        case 1:
        //            playerItem.Down.Images.Add(ExtractBackground(new BitmapImage(
        //                new Uri(playersImagePath)), TexturesPosition.Player1StartImageX, TexturesPosition.Player1StartImageY, TexturesPosition.PlayerWidth, TexturesPosition.PlayerHeight));
        //            break;
        //    }
        //}

        private static void ExtractDown(PlayerItem player, int playerNumber, BitmapImage imageBmp, int imageNumber, int imageDirectionStart)
        {
            //posX = start + Image direction start * number of image * (player width + space between them)
            int posX = TexturesPosition.PlayerStartImageX + (imageDirectionStart*imageNumber*(TexturesPosition.PlayerWidth + TexturesPosition.SpaceBetweenImages));
            //posY = start + player number * (playerHeight  + space between them)
            int posY = TexturesPosition.PlayerStartImageY + playerNumber*(TexturesPosition.PlayerHeight + TexturesPosition.SpaceBetweenImages);

            Brush brushLeft = ExtractBackground(imageBmp, posX, posY, TexturesPosition.PlayerWidth, TexturesPosition.PlayerHeight);

            if (imageNumber == MiddleImage)
                player.ImageInUse = player.Down.Images[1];

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
        
        #endregion
    }

    public class StartedGameViewModelDesignData : StartedGameViewModel
    {
        public StartedGameViewModelDesignData()
        {
            MapViewModel = new MapViewModelDesignData();
        }
    }
}
