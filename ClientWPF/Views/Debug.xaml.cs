using System.ComponentModel;
using System.Windows.Controls;
using ClientWPF.Helpers;
using ClientWPF.ViewModels;

namespace ClientWPF.Views
{
    /// <summary>
    /// Interaction logic for Debug.xaml
    /// </summary>
    public partial class Debug : UserControl
    {
        public DebugViewModel DebugViewModel;

        public Debug()
        {
            ExecuteOnUIThread.Initialize();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DebugViewModel = new DebugViewModel();
                DataContext = DebugViewModel;
                DebugViewModel.Initialize();
            }
            Focusable = true;
        }
    }
}
