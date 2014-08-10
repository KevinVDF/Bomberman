using System.Collections.ObjectModel;
using System.Configuration;
using Common.DataContract;

namespace ClientWPF.ViewModels.StartedGame
{
    public class StartedGameViewModel : ViewModelBase
    {
        #region Properties

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

        public string GlobalImagePath { get; set; }

        #endregion 

        //Constructor
        public StartedGameViewModel()
        {
            MapViewModel = new MapViewModel();
        }

        #region Methods

        public void Initialize(bool isVisible)
        {
            IsVisible = isVisible;
            GlobalImagePath = ConfigurationManager.AppSettings["ImagePath"];
        }

        public void RegisterGame(Game newGame)
        {
            int playerNumber = 0;

            ObservableCollection<LivingObjectItem> livingObjectItems = new ObservableCollection<LivingObjectItem>();

            Textures.Textures.InitializeItem();

            foreach (LivingObject livingObject in newGame.Map.GridPositions)
            {
                LivingObjectItem livingObjectItem = null;
                if(livingObject is Wall)
                    livingObjectItem = MapToWallItem(livingObject as Wall);
                if(livingObject is Player)
                    livingObjectItem = MapToPlayerItem(livingObject as Player, playerNumber++);
                livingObjectItems.Add(livingObjectItem);
            }
            MapViewModel.LivingObjects = livingObjectItems;
        }

        private static LivingObjectItem MapToPlayerItem(Player player, int playerNumber)
        {

            PlayerItem playerItem = new PlayerItem
            {
                PositionX = player.ObjectPosition.PositionX,
                PositionY = player.ObjectPosition.PositionY,
                Id = player.Id
            };
            playerNumber++;
            switch (playerNumber)
            {
                case 1:
                    playerItem.Textures = Textures.Textures.Player1Item.Textures;
                    playerItem.ImageInUse = Textures.Textures.Player1Item.ImageInUse;
                    break;
                case 2:
                    playerItem.Textures = Textures.Textures.Player2Item.Textures;
                    playerItem.ImageInUse = Textures.Textures.Player2Item.ImageInUse;
                    break;
                case 3:
                    playerItem.Textures = Textures.Textures.Player3Item.Textures;
                    playerItem.ImageInUse = Textures.Textures.Player3Item.ImageInUse;
                    break;
                case 4:
                    playerItem.Textures = Textures.Textures.Player4Item.Textures;
                    playerItem.ImageInUse = Textures.Textures.Player4Item.ImageInUse;
                    break;
            }
            return playerItem;
        }

        private LivingObjectItem MapToWallItem(Wall wall)
        {
            WallItem wallItem = new WallItem
            {
                PositionX = wall.ObjectPosition.PositionX,
                PositionY = wall.ObjectPosition.PositionY,
                WallType = wall.WallType,
                Textures =
                    wall.WallType == WallType.Destructible
                        ? Textures.Textures.DestructibleWallItem.Textures
                        : Textures.Textures.UndestructibleWallItem.Textures,
                ImageInUse = 
                    wall.WallType == WallType.Destructible
                        ? Textures.Textures.DestructibleWallItem.ImageInUse
                        : Textures.Textures.UndestructibleWallItem.ImageInUse,
            };
            return wallItem;
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            MapViewModel.OnPlayerMove(player, newPosition, actionType);
        }

        public void OnBombDropped(Bomb newBomb)
        {
            MapViewModel.OnBombDropped(newBomb);
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
