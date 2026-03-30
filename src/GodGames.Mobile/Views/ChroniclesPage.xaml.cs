using GodGames.Mobile.ViewModels;

namespace GodGames.Mobile.Views;

public partial class ChroniclesPage : ContentPage
{
    private readonly ChroniclesViewModel _vm;

    public ChroniclesPage(ChroniclesViewModel vm)
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
