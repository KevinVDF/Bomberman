﻿using ClientWPF.ViewModels.GameRoom;

namespace ClientWPF.Views.GameRoom
{
    /// <summary>
    /// Interaction logic for ucGameRoom.xaml
    /// </summary>
    public partial class ucGameRoom
    {
        public ucGameRoom()
        {
            GameRoomViewModel gameRoomViewModel = new GameRoomViewModel();
            DataContext = gameRoomViewModel;
            InitializeComponent();
        }
    }
}
