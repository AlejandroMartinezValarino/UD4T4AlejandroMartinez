using UD4T4AlejandroMartinez.MVVM.Views;

namespace UD4T4AlejandroMartinez
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new NavigationPage(new Login());
        }
    }
}
