using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Models;
using GodGames.Mobile.Services;
using System.Collections.ObjectModel;

namespace GodGames.Mobile.ViewModels;

public partial class WorldMapViewModel(GodGamesApiClient api) : BaseViewModel
{
    [ObservableProperty] private ChampionDto? _champion;
    [ObservableProperty] private WorldRegionDto? _selectedRegion;
    [ObservableProperty] private bool _isLoading = true;

    public ObservableCollection<WorldRegionDto> Regions { get; } = [];

    public string CurrentRegionId => Champion?.CurrentRegionId ?? string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var regionsTask = api.GetWorldRegionsAsync();
        var championTask = api.GetMyChampionAsync();
        await Task.WhenAll(regionsTask, championTask);

        Regions.Clear();
        foreach (var r in regionsTask.Result) Regions.Add(r);
        Champion = championTask.Result;
        IsLoading = false;
        OnPropertyChanged(nameof(CurrentRegionId));
    }
}
