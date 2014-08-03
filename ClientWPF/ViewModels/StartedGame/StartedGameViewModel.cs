using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.ViewModels.StartedGame
{
    public class StartedGameViewModel : ViewModelBase
    {
        #region Properties

        public static IBombermanService Proxy;

        public Game Game { get; set; }

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
        }

        #endregion
    }
}
