using GodGames.Mobile.Services;

namespace GodGames.Mobile;

public partial class App : Application
{
    public App(AuthService auth)
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
