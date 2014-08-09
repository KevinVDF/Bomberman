using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Input;
using ClientWPF.Logic;
using ClientWPF.MVVM;
using Common.Interfaces;

namespace ClientWPF.ViewModels.GameRoom
{
    public class GameRoomViewModel : ViewModelBase
    {

        #region Properties

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
            get
            {
                _startGameCommand = _startGameCommand ?? new RelayCommand(StartGame);
                return _startGameCommand;
            }
        }

        #endregion Properties

        #region Methods

        public void StartGame()
        {
            ClientModel.StartGame();
        }

        public void Initialize(bool isLogged)
        {
            IsVisible = isLogged;
        }

        public void GenerateTextOnConnection(string myUsername, List<string> loginsList, bool isCreator)
        {
            string richText = "------------------------------------------------\n";
            richText += "---------- Welcome to Bomberman ----------\n";
            richText += "-----------------" + myUsername + "---------------\n\n\n";
            richText += "           List of players online\n\n";
            richText += "__________________________________________\n\n\n";
            richText = loginsList.Aggregate(richText, (current, login) => current + (login + "\n\n"));
            if (isCreator)
            {
                richText += loginsList.Count() > 1 ? "Click button !!": "Wait for other players...";
            }
            else richText += "Wait until the creator start the game.";
            RichText = richText;
        }

        #endregion Methods
    }

    public class GameRoomViewModelDesignData : GameRoomViewModel
    {
        public GameRoomViewModelDesignData()
        {
            RichText = "------------------------------------------------\n";
            RichText += "---------- Welcome to Bomberman ----------\n";
            RichText += "----------------- test ---------------\n\n\n";
            RichText += "         List of players online\n";
            RichText += "__________________________________________\n\n\n";
            IsStartVisible = true;
        }
    }
}
