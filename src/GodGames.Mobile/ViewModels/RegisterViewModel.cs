using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Services;

namespace GodGames.Mobile.ViewModels;

public partial class RegisterViewModel(AuthService auth) : BaseViewModel
{
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _success;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        var error = await auth.RegisterAsync(Email, Password);
        IsBusy = false;

        if (error is null) Success = true;
        else ErrorMessage = error;
    }

    [RelayCommand]
    private static async Task GoToLoginAsync() =>
        await Shell.Current.GoToAsync("//login");
}
