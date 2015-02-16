using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common.DataContract;

namespace ClientWPF.ViewModels.StartedGame
{
    public class MapViewModel : ViewModelBase
    {
        #region Properties

        private List<Timer> _timers; 

        private ObservableCollection<LivingObjectItem> _livingObjects; 
        public ObservableCollection<LivingObjectItem> LivingObjects
        {
            get { return _livingObjects; } 
            set { Set(() => LivingObjects, ref _livingObjects, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { Set(() => IsEnabled, ref _isEnabled, value); }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { Set(() => Width, ref _width, value); }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { Set(() => Height, ref _height, value); }
        }

        private bool _endFlag;
        public bool EndFlag
        {
            get { return _endFlag; }
            set { Set(() => EndFlag, ref _endFlag, value); }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            EndFlag = false;
            IsEnabled = true;
            _timers = new List<Timer>();
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {

            PlayerItem objectToMove = LivingObjects.FirstOrDefault(x => x.X == (player.Position.X) && x.Y == player.Position.Y
                                                                    && x is PlayerItem && player.Id == ((PlayerItem)x).Id ) as PlayerItem;
            if (objectToMove != null)
            {
                objectToMove.X = newPosition.X;
                objectToMove.Y = newPosition.Y;

                switch (actionType)
                {
                    case ActionType.MoveDown:
                        objectToMove.ImageInUse = objectToMove.Textures.Down[1];
                        break;
                    case ActionType.MoveLeft:
                        objectToMove.ImageInUse = objectToMove.Textures.Left[1];
                        break;
                    case ActionType.MoveRight:
                        objectToMove.ImageInUse = objectToMove.Textures.Right[1];
                        break;
                    case ActionType.MoveUp:
                        objectToMove.ImageInUse = objectToMove.Textures.Up[1];
                        break;
                }
            }
        }

        public void OnBombDropped(Bomb newBomb)
        {
            BombItem bomb = MapToBombItem(newBomb) as BombItem;
            LivingObjects.Add(bomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            //search the bomb associated to the view with the model id
            BombItem bombItem = LivingObjects.First(x => x is BombItem && x.ID == bomb.ID) as BombItem;
=======
=======
>>>>>>> origin/master
            BombItem bombItem = LivingObjects.FirstOrDefault(x => x is BombItem && ((BombItem) x).Id == bomb.Id) as BombItem;

            if (bombItem != null)
            {
                bombItem.ImageInUse = Textures.Textures.ExplodedBombItem.ImageInUse;
                bombItem.Height = 150;
                bombItem.Width = 150;
                Timer t = new Timer(BombExploded, bombItem, 500, Timeout.Infinite);
                _timers.Add(t);
<<<<<<< HEAD
>>>>>>> origin/master
=======
>>>>>>> origin/master
=======
            BombItem bombItem = LivingObjects.First(x => x is BombItem && ((BombItem) x).Id == bomb.Id) as BombItem;
>>>>>>> parent of eeb1811... rewrite model objects

            if (bombItem != null)
            {
                bombItem.ImageInUse = Textures.Textures.ExplodedBombItem.ImageInUse;
                bombItem.Height = 150;
                bombItem.Width = 150;
                Timer t = new Timer(BombExploded, bombItem, 500, Timeout.Infinite);

                foreach (LivingObject livingObject in impacted)
                {
                    if (livingObject is Wall)
                    {
                        LivingObjectItem wallToRemove = LivingObjects.First(x => x is WallItem && ((WallItem)x).Id == ((Wall)livingObject).Id);
                        LivingObjects.Remove(wallToRemove);
                    }
                        
                    if (livingObject is Player)
                    {
                        LivingObjectItem playerToRemove = LivingObjects.FirstOrDefault(x => x is PlayerItem && ((PlayerItem) x).Id == ((Player) livingObject).Id);
                        LivingObjects.Remove(playerToRemove);
                    }
                }
            }
        }

        private void BombExploded(object bomb)
        {
            if(_timers.Any())
                _timers.RemoveAt(0);
            ExecuteOnUIThread(() => LivingObjects.Remove(bomb as BombItem));
        }

        private LivingObjectItem MapToBombItem(Bomb bomb)
        {
            BombItem bombitem = new BombItem
            {
                Id = bomb.Id,
                X = bomb.Position.X,
                Y = bomb.Position.Y,
                PlayerId = bomb.PlayerId,
                Power = bomb.Power,
                ImageInUse = Textures.Textures.BombItem.ImageInUse,
                ZIndex = 1,
                Width = 20,
                Height = 25
            };
            return bombitem;
        }

        #endregion

    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
