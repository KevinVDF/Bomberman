using ClientWPF.ViewModels.Login;

namespace ClientWPF.Views.Login
{
    /// <summary>
    /// Interaction logic for ucLogin.xaml
    /// </summary>
    public partial class ucLogin
    {
        public ucLogin()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            DataContext = loginViewModel;
            InitializeComponent();
        }
    }
}
