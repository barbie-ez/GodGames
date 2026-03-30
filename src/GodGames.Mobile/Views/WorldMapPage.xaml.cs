using GodGames.Mobile.ViewModels;

namespace GodGames.Mobile.Views;

public partial class WorldMapPage : ContentPage
{
    private readonly WorldMapViewModel _vm;

    public WorldMapPage(WorldMapViewModel vm)
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
