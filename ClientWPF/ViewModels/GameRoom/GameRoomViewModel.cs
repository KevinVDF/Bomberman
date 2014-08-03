using System.Collections.Generic;
using System.Collections.ObjectModel;
<<<<<<< HEAD
using System.Linq;
using System.Windows.Input;
using ClientWPF.MVVM;
using Common.DataContract;
using Common.Interfaces;
=======
using System.ComponentModel;
using System.Configuration;
using System.Windows.Documents;
using System.Windows.Input;
using Common.DataContract;
>>>>>>> origin/master

namespace ClientWPF.ViewModels.GameRoom
{
    public class GameRoomViewModel : ViewModelBase
    {
<<<<<<< HEAD
        public static IBombermanService Proxy { get; set; }

        public const string MapPath = @"C:\Users\hisil\HDD backup 2012\Bomberman\Server\map.dat";

        public Player Player { get; set; }

        public ObservableCollection<string> PlayerList { get; set; }
=======
        public ObservableCollection<string> PlayerList { get; 
            set;
        }
>>>>>>> origin/master

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
<<<<<<< HEAD
            get
            {
                _startGameCommand = _startGameCommand ?? new RelayCommand(StartGame);
                return _startGameCommand;
            }
        }

        public void StartGame()
        {
            Proxy.StartGame(MapPath);
        }

        public void Initialize(bool isLogged, string text, IBombermanService proxy)
        {
            IsVisible = isLogged;
            RichText = text;
            Proxy = proxy;
        }

        public void GenerateGameRoomText(Player newPlayer, List<string> loginsList, bool canStartGame)
        {
            string richText = "------------------------------------------------\n";
            richText += "------------- Welcome to Bomberman -------------\n";
            richText += "-----------------" + Player.Username + "---------------\n\n";
            richText += "New User Joined the server : " + newPlayer.Username + "\n";
            richText += "List of players online :\n\n";
            richText = loginsList.Aggregate(richText, (current, login) => current + (login + "\n\n"));
            if (Player.IsCreator)
            {
                richText += canStartGame ? "Click button !!" : "Wait for other players...";
            }
            else richText += "Wait until the creator start the game.";
            RichText = richText;
=======
            get { return _startGameCommand; } 
            set { Set(() => StartGameCommand, ref _startGameCommand, value); }
        }

        public void Initialize(bool isLogged, string text)
        {
            IsVisible = isLogged;
            RichText = text;
>>>>>>> origin/master
        }
    }

    public class GameRoomViewModelDesignData : GameRoomViewModel
    {
        
    }
}
