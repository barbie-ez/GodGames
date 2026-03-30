using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Models;
using GodGames.Mobile.Services;
using System.Collections.ObjectModel;

namespace GodGames.Mobile.ViewModels;

public partial class ChroniclesViewModel(GodGamesApiClient api) : BaseViewModel
{
    [ObservableProperty] private ChampionDto? _champion;
    [ObservableProperty] private bool _isLoading = true;

    public ObservableCollection<NarrativeEntryDto> Entries { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        Champion = await api.GetMyChampionAsync();
        var entries = await api.GetNarrativesAsync(count: 50);
        Entries.Clear();
        foreach (var e in entries) Entries.Add(e);
        IsLoading = false;
    }
}
