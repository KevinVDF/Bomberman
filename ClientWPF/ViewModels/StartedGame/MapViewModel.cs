using System.Collections.ObjectModel;
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

        public static IBombermanService Proxy { get; set; }

        public Player Player { get; set; }

        #endregion

        #region Methods

        //Constructor
        public void Initialize(IBombermanService proxy)
        {
            Proxy = proxy;
        }

        public void InitializePlayer(Player player)
        {
            Player = player;
        }

        #endregion
    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
