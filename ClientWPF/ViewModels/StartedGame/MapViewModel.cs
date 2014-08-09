using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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

        public void OnPlayerMove(Player player, Position newPosition)
        {
            PlayerItem objectToMove = LivingObjects.FirstOrDefault(x => x.PositionX == (player.ObjectPosition.PositionX * 50) + 100 && x.PositionY == player.ObjectPosition.PositionY * 50) as PlayerItem;
            if (objectToMove != null)
            {
                objectToMove.PositionX = newPosition.PositionX;
                objectToMove.PositionY = newPosition.PositionY;
            }
        }

        private static void MovePlayer(PlayerItem objectToMove, Player playerAfter)
        {
            objectToMove.PositionX = playerAfter.ObjectPosition.PositionX;
            objectToMove.PositionY = playerAfter.ObjectPosition.PositionY;
        }
        #endregion
        
    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
