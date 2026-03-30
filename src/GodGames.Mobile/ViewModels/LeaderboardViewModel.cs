using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Models;
using GodGames.Mobile.Services;
using System.Collections.ObjectModel;

namespace GodGames.Mobile.ViewModels;

public partial class LeaderboardViewModel(GodGamesApiClient api) : BaseViewModel
{
    [ObservableProperty] private bool _isLoading = true;
    public ObservableCollection<LeaderboardEntryDto> Entries { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        var entries = await api.GetLeaderboardAsync();
        Entries.Clear();
        foreach (var e in entries) Entries.Add(e);
        IsLoading = false;
    }
}
