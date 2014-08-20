using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Windows.Input;
using ClientWPF.Logic;
using ClientWPF.MVVM;
using ClientWPF.Proxies;
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

        private bool _isRestartVisible;
        public bool IsRestartVisible
        {
            get { return _isRestartVisible; }
            set { Set(() => IsRestartVisible, ref _isRestartVisible, value); }
        }

        private string _infoLabel;
        public string InfoLabel
        {
            get { return _infoLabel; }
            set { Set(() => InfoLabel, ref _infoLabel, value); }
        }        

        private ICommand _restartGameCOmmand;
        public ICommand RestartGameCommand
        {
            get
            {
                _restartGameCOmmand = _restartGameCOmmand ?? new RelayCommand(RestartGame);
                return _restartGameCOmmand;
            } 
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
            IsRestartVisible = false;
            MapViewModel.Initialize();
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
            MapViewModel.Width = 50*newGame.Map.MapSize;
            MapViewModel.Height = 50 * newGame.Map.MapSize;
        }

        private static LivingObjectItem MapToPlayerItem(Player player, int playerNumber)
        {

            PlayerItem playerItem = new PlayerItem
            {
                X = player.Position.X,
                Y = player.Position.Y,
                Id = player.Id,
                ZIndex = 500,
                Width = 30,
                Height = 35
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
                X = wall.Position.X,
                Y = wall.Position.Y,
                WallType = wall.WallType,
                Textures =
                    wall.WallType == WallType.Destructible
                        ? Textures.Textures.DestructibleWallItem.Textures
                        : Textures.Textures.UndestructibleWallItem.Textures,
                ImageInUse =
                    wall.WallType == WallType.Destructible
                        ? Textures.Textures.DestructibleWallItem.ImageInUse
                        : Textures.Textures.UndestructibleWallItem.ImageInUse,
                ZIndex = 200,
                Height = 50,
                Width = 50,
                Id = wall.Id
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

        public void OnBombExploded(Bomb bomb, System.Collections.Generic.List<LivingObject> impacted)
        {
            MapViewModel.OnBombExploded(bomb, impacted);
        }

        public void DisplayMessage(string message)
        {
            InfoLabel += "\n" + message;
        }

        public void OnCanRestart()
        {
            IsRestartVisible = true;
            MapViewModel.IsEnabled = false;
        }

        private void RestartGame()
        {
            ClientModel.RestartGame();
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
