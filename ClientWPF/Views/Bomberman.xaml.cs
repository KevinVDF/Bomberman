﻿using System.ComponentModel;
using System.Windows.Input;
using ClientWPF.Helpers;
using ClientWPF.ViewModels;
using Common.DataContract;

namespace ClientWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Bomberman
    {

        public BombermanViewModel BombermanViewModel { get; set; }

        public Bomberman()
        {
            ExecuteOnUIThread.Initialize();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                BombermanViewModel = new BombermanViewModel();
                DataContext = BombermanViewModel;
                BombermanViewModel.Initialize();
            }
            Focusable = true;
        }

        private void Bomberman_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    BombermanViewModel.PlayerAction(ActionType.MoveLeft);
                    break;
                case Key.Right:
                    BombermanViewModel.PlayerAction(ActionType.MoveRight);
                    break;
                case Key.Up:
                    BombermanViewModel.PlayerAction(ActionType.MoveUp);
                    break;
                case Key.Down:
                    BombermanViewModel.PlayerAction(ActionType.MoveDown);
                    break;
                case Key.Space:
                    BombermanViewModel.PlayerAction(ActionType.DropBomb);
                    break;
            }
        }
    }
}
