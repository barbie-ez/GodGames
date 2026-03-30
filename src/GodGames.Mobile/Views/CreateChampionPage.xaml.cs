using GodGames.Mobile.ViewModels;

namespace GodGames.Mobile.Views;

public partial class CreateChampionPage : ContentPage
{
    public CreateChampionPage(CreateChampionViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
