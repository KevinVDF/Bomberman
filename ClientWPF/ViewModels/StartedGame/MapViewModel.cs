using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.ViewModels.StartedGame
{
    public class MapViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<LivingObjectItem> _livingObjects; 
        public ObservableCollection<LivingObjectItem> LivingObjects
        {
            get { return _livingObjects; } 
            set { Set(() => LivingObjects, ref _livingObjects, value); }
        }

        #endregion

        #region Methods

        public void Initialize(IBombermanService proxy)
        {
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {
            //TODO MODIFY magic number bye constants
            PlayerItem objectToMove = LivingObjects.FirstOrDefault(x => x.PositionX == (player.ObjectPosition.PositionX * 50) + 100 && x.PositionY == player.ObjectPosition.PositionY * 50
                && x is PlayerItem && player.Id == ((PlayerItem)x).Id ) as PlayerItem;
            if (objectToMove != null)
            {
                objectToMove.PositionX = newPosition.PositionX;
                objectToMove.PositionY = newPosition.PositionY;

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
            BombItem bomb = new BombItem
            {
                PositionX = newBomb.ObjectPosition.PositionX,
                PositionY = newBomb.ObjectPosition.PositionY,
                Power = newBomb.Power,
                ImageInUse = Textures.Textures.BombItem.ImageInUse
            };

            LivingObjects.Add(bomb);
        }
        
        #endregion

    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
