using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Documents;
using System.Windows.Input;
using Common.DataContract;

namespace ClientWPF.ViewModels.GameRoom
{
    public class GameRoomViewModel : ViewModelBase
    {
        public ObservableCollection<string> PlayerList { get; 
            set;
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(() => IsVisible, ref _isVisible, value); }
        }

        private string _richText;
        public string RichText
        {
            get { return _richText; }
            set { Set(() => RichText, ref _richText, value); }
        }

        private bool _isStartVisible;
        public bool IsStartVisible
        {
            get { return _isStartVisible; }
            set { Set(() => IsStartVisible, ref _isStartVisible, value); }
        }

        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; } 
            set { Set(() => IsStartEnabled, ref _isStartEnabled, value); }
        }

        private ICommand _startGameCommand;
        public ICommand StartGameCommand
        {
            get { return _startGameCommand; } 
            set { Set(() => StartGameCommand, ref _startGameCommand, value); }
        }

        public void Initialize(bool isLogged, string text)
        {
            IsVisible = isLogged;
            RichText = text;
        }
    }

    public class GameRoomViewModelDesignData : GameRoomViewModel
    {
        
    }
}
