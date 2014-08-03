using System;
using System.Windows.Input;
using ClientWPF.MVVM;
using Common.Interfaces;

namespace ClientWPF.ViewModels.Login
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master
        public static IBombermanService Proxy { get; set; }

        public string Login { get; set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(() => IsVisible, ref _isVisible, value); }
        }

<<<<<<< HEAD
=======
=======
        public IBombermanService Proxy { get; private set; }

        public string Login { get; set; }

>>>>>>> origin/master
>>>>>>> origin/master
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                _connectCommand = _connectCommand ?? new RelayCommand(Connect);
                return _connectCommand;
            }
        }

        #endregion

        #region Methods

        private void Connect()
        {
            int id = Guid.NewGuid().GetHashCode();
            Proxy.ConnectUser(Login, id);
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master
        public void Initialize(bool isVisible, IBombermanService proxy)
        {
            Proxy = proxy;
            IsVisible = isVisible;
<<<<<<< HEAD
=======
=======
        public void Initialize(IBombermanService proxy)
        {
            Proxy = proxy;
>>>>>>> origin/master
>>>>>>> origin/master
        }

        #endregion
    }

    public class LoginViewModelDesignData : LoginViewModel
    {
        public LoginViewModelDesignData()
        {
            Login = "Test";
        }
    }
}
