using System.Net.Http.Json;
using GodGames.Mobile.Models;

namespace GodGames.Mobile.Services;

public class AuthService(HttpClient http)
{
    private const string TokenKey = "gg_jwt";

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/auth/login", new LoginRequest(email, password));
            if (!response.IsSuccessStatusCode)
                return (false, $"HTTP {(int)response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null) return (false, "Empty response from server.");

            await SecureStorage.Default.SetAsync(TokenKey, result.Token);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<string?> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/auth/register", new RegisterRequest(email, password));
            if (response.IsSuccessStatusCode) return null;

            var errors = await response.Content.ReadFromJsonAsync<string[]>();
            return errors is { Length: > 0 } ? string.Join(" ", errors) : $"Registration failed ({(int)response.StatusCode}).";
        }
        catch (Exception ex) { return ex.Message; }
    }

    public Task<string?> GetTokenAsync() =>
        SecureStorage.Default.GetAsync(TokenKey);

    public void Logout() => SecureStorage.Default.Remove(TokenKey);

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
