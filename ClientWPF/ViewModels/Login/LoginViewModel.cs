using System;
using System.Windows.Input;
using ClientWPF.MVVM;
using Common.Interfaces;

namespace ClientWPF.ViewModels.Login
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties

        public static IBombermanService Proxy { get; set; }

        public string Login { get; set; }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(() => IsVisible, ref _isVisible, value); }
        }

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
            Proxy.ConnectUser(Login);
        }

        public void Initialize(bool isVisible, IBombermanService proxy)
        {
            Proxy = proxy;
            IsVisible = isVisible;
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
