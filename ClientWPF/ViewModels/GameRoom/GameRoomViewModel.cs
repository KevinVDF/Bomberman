using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ClientWPF.MVVM;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.ViewModels.GameRoom
{
    public class GameRoomViewModel : ViewModelBase
    {
        public static IBombermanService Proxy { get; set; }

        public const string MapPath = @"C:\Users\hisil\HDD backup 2012\Bomberman\Server\map.dat";

        public Player Player { get; set; }

        public ObservableCollection<string> PlayerList { get; set; }

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
        }
    }

    public class GameRoomViewModelDesignData : GameRoomViewModel
    {
        
    }
}
