using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common.DataContract;

namespace ClientWPF.ViewModels.StartedGame
{
    public class MapViewModel : ViewModelBase
    {
        #region Variables

        private readonly List<Timer> _timers = new List<Timer>(); // SinaC: should learn what GC is :)

        #endregion 

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

            PlayerItem objectToMove = LivingObjects.First(x => x.X == (player.Position.X) && x.Y == player.Position.Y
                                                                    && x is PlayerItem && player.ID == x.ID ) as PlayerItem;
            if (objectToMove == null) 
                return;
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

        public void OnBombDropped(Bomb newBomb)
        {
            BombItem bomb = MapToBombItem(newBomb) as BombItem;
            LivingObjects.Add(bomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
<<<<<<< HEAD
            //search the bomb associated to the view with the model id
            BombItem bombItem = LivingObjects.First(x => x is BombItem && x.ID == bomb.ID) as BombItem;
=======
            BombItem bombItem = LivingObjects.FirstOrDefault(x => x is BombItem && ((BombItem) x).Id == bomb.Id) as BombItem;

            if (bombItem != null)
            {
                bombItem.ImageInUse = Textures.Textures.ExplodedBombItem.ImageInUse;
                bombItem.Height = 150;
                bombItem.Width = 150;
                Timer t = new Timer(BombExploded, bombItem, 500, Timeout.Infinite);
                _timers.Add(t);
>>>>>>> origin/master

            if (bombItem == null) 
                return;
            ExecuteOnUIThread(() =>
                {
                    //change image to have an exploded bomb 
                    bombItem.ImageInUse = Textures.Textures.DestructibleWallItem.ImageInUse; //Textures.Textures.ExplodedBombItem.ImageInUse;
                    bombItem.Height = 150;
                    bombItem.Width = 150;
                    bombItem.ZIndex = 1000;
                });

            Timer t = new Timer(BombExploded, bombItem, 1500, Timeout.Infinite);
            _timers.Add(t);

            if (impacted == null || !impacted.Any())
                return;
            //Search all objects associated to the view matching with id's of impacted objects
            foreach (LivingObject livingObject in impacted)
            {
                LivingObjectItem objectToRemove = LivingObjects.First(x => x.ID == livingObject.ID);
                LivingObjects.Remove(objectToRemove);
            }
            LivingObjects.Remove(bombItem);
        }

        private void BombExploded(object bomb)
        {
            if(_timers.Any())
                _timers.RemoveAt(0);
            ExecuteOnUIThread(() => LivingObjects.Remove(bomb as BombItem));
        }

        private LivingObjectItem MapToBombItem(Bomb bomb)
        {
            BombItem bombItem = new BombItem
            {
                ID = bomb.ID,
                X = bomb.Position.X,
                Y = bomb.Position.Y,
                PlayerId = bomb.PlayerId,
                Power = bomb.Power,
                ImageInUse = Textures.Textures.BombItem.ImageInUse,
                ZIndex = 1,
                Width = 20,
                Height = 25
            };
            return bombItem;
        }

        #endregion

    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
