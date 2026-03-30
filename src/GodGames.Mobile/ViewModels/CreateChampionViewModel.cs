using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Models;
using GodGames.Mobile.Services;

namespace GodGames.Mobile.ViewModels;

public partial class CreateChampionViewModel(GodGamesApiClient api) : BaseViewModel
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private ChampionClass _selectedClass = ChampionClass.Warrior;
    [ObservableProperty] private PersonalityTrait _selectedTrait = PersonalityTrait.Brave;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public List<string> Classes { get; } = ["Warrior", "Mage", "Rogue"];
    public List<string> Traits { get; } = ["Brave", "Cautious", "Reckless", "Cunning"];

    public int SelectedClassIndex
    {
        get => (int)SelectedClass;
        set => SelectedClass = (ChampionClass)value;
    }

    public int SelectedTraitIndex
    {
        get => (int)SelectedTrait;
        set => SelectedTrait = (PersonalityTrait)value;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Enter a champion name."; return; }
        IsBusy = true;
        ErrorMessage = string.Empty;

        var champion = await api.CreateChampionAsync(new CreateChampionRequest(Name, SelectedClass, SelectedTrait));
        IsBusy = false;

        if (champion is not null)
            await Shell.Current.GoToAsync("//dashboard");
        else
            ErrorMessage = "Failed to create champion. You may already have one.";
    }
}
