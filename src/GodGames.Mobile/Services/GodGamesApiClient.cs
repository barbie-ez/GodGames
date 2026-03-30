using System.Net.Http.Json;
using GodGames.Mobile.Models;

namespace GodGames.Mobile.Services;

public class GodGamesApiClient(HttpClient http, AuthService auth)
{
    public async Task<ChampionDto?> GetMyChampionAsync()
    {
        var req = await BuildRequest(HttpMethod.Get, "api/champions/me");
        var res = await http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ChampionDto>() : null;
    }

    public async Task<ChampionDto?> CreateChampionAsync(CreateChampionRequest body)
    {
        var req = await BuildRequest(HttpMethod.Post, "api/champions");
        req.Content = JsonContent.Create(body);
        var res = await http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ChampionDto>() : null;
    }

    public async Task<bool> SubmitInterventionAsync(SubmitInterventionRequest body)
    {
        var req = await BuildRequest(HttpMethod.Post, "api/interventions");
        req.Content = JsonContent.Create(body);
        var res = await http.SendAsync(req);
        return res.IsSuccessStatusCode;
    }

    public async Task<List<NarrativeEntryDto>> GetNarrativesAsync(int count = 10)
    {
        var req = await BuildRequest(HttpMethod.Get, $"api/champions/me/narratives?count={count}");
        var res = await http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<NarrativeEntryDto>>() ?? [] : [];
    }

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync()
    {
        var res = await http.GetAsync("api/champions/leaderboard");
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<LeaderboardEntryDto>>() ?? [] : [];
    }

    public async Task<List<WorldRegionDto>> GetWorldRegionsAsync()
    {
        var res = await http.GetAsync("api/regions");
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<WorldRegionDto>>() ?? [] : [];
    }

    private async Task<HttpRequestMessage> BuildRequest(HttpMethod method, string url)
    {
        var msg = new HttpRequestMessage(method, url);
        var token = await auth.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            msg.Headers.Authorization = new("Bearer", token);
        return msg;
    }
}
