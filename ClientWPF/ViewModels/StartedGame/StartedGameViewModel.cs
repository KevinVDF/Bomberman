using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientWPF.Model;
using ClientWPF.Textures;
using Common.DataContract;
using Common.Interfaces;
using Common.Log;
using Player = ClientWPF.Model.Player;

namespace ClientWPF.ViewModels.StartedGame
{
    public class StartedGameViewModel : ViewModelBase
    {
        #region Properties

        public string GlobalImagePath { get; set; }

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
                if(livingObject is Wall)
                    livingObjectItem = new Wall
                    {
                        PositionX = livingObject.ObjectPosition.PositionX,
                        PositionY = livingObject.ObjectPosition.PositionY,
                        WallType = ((Wall)livingObject).WallType
                    };
                if (livingObject is Common.DataContract.Player)
                {
                    livingObjectItem = new Player
                    {
                        PositionX = livingObject.ObjectPosition.PositionX,
                        PositionY = livingObject.ObjectPosition.PositionY,
                    };
                    playerNumber++;
                    GetSpriteForPlayer(livingObjectItem as Player, playerNumber);
                }
                
                livingObjectItems.Add(livingObjectItem);
            }
            MapViewModel.LivingObjects = livingObjectItems;
        }

        private void GetSpriteForPlayer(Player player, int playerNumber)
        {

            string playersImagePath = String.Format("{0}/players.png", GlobalImagePath);
            
            for (int i = 0; i <= TexturesPosition.NumberImagesPerSpriteByPlayer; i++)
            {
                //down
                ExtractDown(player, playerNumber, playersImagePath, i, TexturesPosition.DownImagePosition);
                ////left
                //ExtractLeft(playerItem, playerNumber, playersImagePath);
                ////right
                //ExtractRight(playerItem, playerNumber, playersImagePath);
                ////up
                //ExtractUp(playerItem, playerNumber, playersImagePath);
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

        private void ExtractDown(Player player, int playerNumber, string playersImagePath, int imageNumber, int imageDirectionStart)
        {
            //posX = start + Image direction start * number of image * (player width + space between them)
            int posX = TexturesPosition.PlayerStartImageX + (imageDirectionStart * imageNumber * (TexturesPosition.PlayerWidth + TexturesPosition.SpaceBetweenImages));
            //posY = start + player number * (playerHeight  + space between them)
            int posY = TexturesPosition.PlayerStartImageY + playerNumber*(TexturesPosition.PlayerHeight + TexturesPosition.SpaceBetweenImages);

            player.Down.Images.Add(ExtractBackground(new BitmapImage(
                    new Uri(playersImagePath)), posX, posY , TexturesPosition.PlayerWidth, TexturesPosition.PlayerHeight));
            
        }

        //private void GetSpriteForWall(LivingObjectItem livingObjectItem, int playerNumber)
        //{

        //    livingObjectItem.Sprite = new Sprite();
        //    if (livingObjectItem is WallItem)
        //    {
        //        if (((WallItem)livingObjectItem).WallType == WallType.Undestructible)
        //        {
        //            livingObjectItem.Sprite.Image1 =
        //                new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Undestructible.png");
        //        }
        //        if (((WallItem)livingObjectItem).WallType == WallType.Destructible)
        //        {
        //            livingObjectItem.Sprite.Image1 =
        //                new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Destructible.png");
        //        }
        //    }
        //    else if (livingObjectItem is PlayerItem)
        //    {

        //        livingObjectItem.Sprite.Image1 = new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Player" + playerNumber + ".png");
        //    }

        //}

        private static Brush ExtractBackground(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.None
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
