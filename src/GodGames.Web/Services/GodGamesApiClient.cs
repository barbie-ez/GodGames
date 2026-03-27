using System.Net.Http.Json;
using GodGames.Web.Auth;
using GodGames.Web.Models;

namespace GodGames.Web.Services;

public class GodGamesApiClient(HttpClient http, GodAuthStateProvider authState)
{
    // Auth
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<LoginResponse>()
            : null;
    }

    /// Returns null on success, or an error message string on failure.
    public async Task<string?> RegisterAsync(RegisterRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", request);
        if (response.IsSuccessStatusCode) return null;

        // Identity returns an array of error description strings
        var errors = await response.Content.ReadFromJsonAsync<string[]>();
        return errors is { Length: > 0 }
            ? string.Join(" ", errors)
            : $"Registration failed ({(int)response.StatusCode}).";
    }

    // Champion
    public async Task<ChampionDto?> GetMyChampionAsync()
    {
        var request = await BuildRequest(HttpMethod.Get, "api/champions/me");
        var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ChampionDto>()
            : null;
    }

    public async Task<ChampionDto?> CreateChampionAsync(CreateChampionRequest body)
    {
        var request = await BuildRequest(HttpMethod.Post, "api/champions");
        request.Content = JsonContent.Create(body);
        var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ChampionDto>()
            : null;
    }

    // Interventions
    public async Task<bool> SubmitInterventionAsync(SubmitInterventionRequest body)
    {
        var request = await BuildRequest(HttpMethod.Post, "api/interventions");
        request.Content = JsonContent.Create(body);
        var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    // Narratives
    public async Task<List<NarrativeEntryDto>> GetNarrativesAsync()
    {
        var request = await BuildRequest(HttpMethod.Get, "api/champions/me/narratives");
        var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<List<NarrativeEntryDto>>() ?? []
            : [];
    }

    private async Task<HttpRequestMessage> BuildRequest(HttpMethod method, string url)
    {
        var message = new HttpRequestMessage(method, url);
        var token = await authState.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            message.Headers.Authorization = new("Bearer", token);
        return message;
    }
}
