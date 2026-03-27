using GodGames.Web.Auth;
using GodGames.Web.Models;

namespace GodGames.Web.Services;

public class AuthService(GodGamesApiClient api, GodAuthStateProvider authState)
{
    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await api.LoginAsync(new LoginRequest(email, password));
        if (response is null) return false;

        await authState.SetTokenAsync(response.Token);
        return true;
    }

    /// Returns null on success, or an error message string on failure.
    public Task<string?> RegisterAsync(string email, string password)
        => api.RegisterAsync(new RegisterRequest(email, password));

    public Task LogoutAsync() => authState.ClearTokenAsync();
}
