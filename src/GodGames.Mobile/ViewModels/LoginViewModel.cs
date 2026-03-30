using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GodGames.Mobile.Services;

namespace GodGames.Mobile.ViewModels;

public partial class LoginViewModel(AuthService auth) : BaseViewModel
{
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        var (ok, error) = await auth.LoginAsync(Email, Password);
        IsBusy = false;

        if (ok)
            await Shell.Current.GoToAsync("//dashboard");
        else
            ErrorMessage = error ?? "Invalid email or password.";
    }

    [RelayCommand]
    private static async Task GoToRegisterAsync() =>
        await Shell.Current.GoToAsync("//register");
}
