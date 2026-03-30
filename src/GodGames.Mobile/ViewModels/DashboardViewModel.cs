using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Models;
using GodGames.Mobile.Services;
using System.Collections.ObjectModel;

namespace GodGames.Mobile.ViewModels;

public partial class DashboardViewModel(GodGamesApiClient api, AuthService auth) : BaseViewModel
{
    [ObservableProperty] private ChampionDto? _champion;
    [ObservableProperty] private string _command = string.Empty;
    [ObservableProperty] private string _commandError = string.Empty;
    [ObservableProperty] private bool _interventionSent;
    [ObservableProperty] private bool _isLoading = true;

    public ObservableCollection<NarrativeEntryDto> Narratives { get; } = [];

    // Computed
    public double HpPercent => Champion is null ? 0 : (double)Champion.HP / Champion.MaxHP;
    public double XpPercent => Champion is null ? 0 : (double)Champion.XP / (Champion.Level * 100);
    public string HpLabel => Champion is null ? "" : $"{Champion.HP} / {Champion.MaxHP}";
    public string XpLabel => Champion is null ? "" : $"{Champion.XP} / {Champion.Level * 100}";
    public string TraitLabel => Champion?.PersonalityTrait.ToString() ?? "";
    public string LevelBadge => Champion is null ? "" : $"Lv. {Champion.Level} — {Champion.Class}";
    public bool HasPowerUp => Champion?.PowerUpSlot is not null;
    public bool HasDebuff => Champion?.ActiveDebuff != DebuffType.None;
    public string PowerUpText => Champion?.PowerUpSlot is null ? "" :
        $"{Champion.PowerUpSlot} — {Champion.PowerUpTicksRemaining} tick{(Champion.PowerUpTicksRemaining == 1 ? "" : "s")} left";
    public string DebuffText => Champion?.ActiveDebuff switch
    {
        DebuffType.StatReduction  => $"Weakened — {Champion.ActiveDebuffTicksRemaining} ticks",
        DebuffType.WeakenedStrike => $"Weakened Strike — {Champion.ActiveDebuffTicksRemaining} ticks",
        DebuffType.CursedBlood    => $"Cursed Blood — {Champion.ActiveDebuffTicksRemaining} ticks",
        _ => ""
    };

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        Champion = await api.GetMyChampionAsync();

        if (Champion is null)
        {
            await Shell.Current.GoToAsync("//createchampion");
            IsLoading = false;
            return;
        }

        var narratives = await api.GetNarrativesAsync();
        Narratives.Clear();
        foreach (var n in narratives) Narratives.Add(n);
        IsLoading = false;

        OnPropertyChanged(nameof(HpPercent));
        OnPropertyChanged(nameof(XpPercent));
        OnPropertyChanged(nameof(HpLabel));
        OnPropertyChanged(nameof(XpLabel));
        OnPropertyChanged(nameof(TraitLabel));
        OnPropertyChanged(nameof(LevelBadge));
        OnPropertyChanged(nameof(HasPowerUp));
        OnPropertyChanged(nameof(HasDebuff));
        OnPropertyChanged(nameof(PowerUpText));
        OnPropertyChanged(nameof(DebuffText));
    }

    [RelayCommand]
    private async Task SubmitInterventionAsync()
    {
        if (string.IsNullOrWhiteSpace(Command) || Champion is null) return;
        IsBusy = true;
        CommandError = string.Empty;

        var ok = await api.SubmitInterventionAsync(new SubmitInterventionRequest(Champion.Id, Command));
        IsBusy = false;

        if (ok) { InterventionSent = true; Command = string.Empty; }
        else CommandError = "Command failed. You may already have a pending intervention this tick.";
    }

    [RelayCommand]
    private static async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }

    partial void OnChampionChanged(ChampionDto? value)
    {
        OnPropertyChanged(nameof(HpPercent));
        OnPropertyChanged(nameof(XpPercent));
        OnPropertyChanged(nameof(HpLabel));
        OnPropertyChanged(nameof(XpLabel));
        OnPropertyChanged(nameof(TraitLabel));
        OnPropertyChanged(nameof(LevelBadge));
        OnPropertyChanged(nameof(HasPowerUp));
        OnPropertyChanged(nameof(HasDebuff));
        OnPropertyChanged(nameof(PowerUpText));
        OnPropertyChanged(nameof(DebuffText));
    }
}
