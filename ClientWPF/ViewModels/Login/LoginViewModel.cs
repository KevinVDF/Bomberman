using System;
using System.Windows.Input;
using ClientWPF.Logic;
using ClientWPF.MVVM;

namespace ClientWPF.ViewModels.Login
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties

        private String _login;
        public string Login
        {
            get { return _login; } 
            set { Set(() => Login, ref _login, value); }
        }

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
            ClientModel.RegisterMe(Login);
        }

        public void Initialize(bool isVisible)
        {
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
