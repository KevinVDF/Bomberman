using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientWPF.Views.StartedGame
{
    /// <summary>
    /// Interaction logic for ucMap.xaml
    /// </summary>
    public partial class ucMap
    {
        public ucMap()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                Background = new SolidColorBrush(Colors.Green);
            else
            {
                ImageBrush ib = new ImageBrush { ImageSource = new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagePath"] + @"\Empty.png", UriKind.Relative)) };
                Background = ib;
            }
        }
    }
}
