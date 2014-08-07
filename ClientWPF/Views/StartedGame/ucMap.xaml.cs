using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientWPF.ViewModels.StartedGame;
using Common.DataContract;

namespace ClientWPF.Views.StartedGame
{
    /// <summary>
    /// Interaction logic for ucMap.xaml
    /// </summary>
    public partial class ucMap : UserControl
    {

        public MapViewModel MapViewModel { get; set; }
        public ucMap()
        {
            InitializeComponent();

            ImageBrush ib = new ImageBrush {ImageSource = new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Empty.png", UriKind.Relative))};
            Background = ib;
        }
    }
}
