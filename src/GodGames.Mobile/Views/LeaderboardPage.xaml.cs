using GodGames.Mobile.ViewModels;

namespace GodGames.Mobile.Views;

public partial class LeaderboardPage : ContentPage
{
    private readonly LeaderboardViewModel _vm;

    public LeaderboardPage(LeaderboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
