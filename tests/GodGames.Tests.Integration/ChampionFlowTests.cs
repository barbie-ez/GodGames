using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GodGames.Tests.Integration;

[Collection("Api")]
public class ChampionFlowTests(GodGamesApiFactory factory)
{
    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private async Task<(HttpClient client, Guid godId)> AuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var email = $"god-{Guid.NewGuid()}@test.com";
        const string password = "Password1";

        await client.PostAsJsonAsync("api/auth/register", new { email, password });
        var loginResp = await client.PostAsJsonAsync("api/auth/login", new { email, password });
        loginResp.EnsureSuccessStatusCode();

        var body = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("token").GetString()!;
        var godId = Guid.Empty; // extracted from register response below

        var registerResp = await client.PostAsJsonAsync("api/auth/register",
            new { email = $"god2-{Guid.NewGuid()}@test.com", password });
        // For godId we re-register a second user — but actually we already registered above.
        // Re-read godId from a re-login isn't directly available; we decode it from the JWT sub claim.
        var jwtParts = token.Split('.');
        var payload = jwtParts[1];
        // Pad base64url
        payload += new string('=', (4 - payload.Length % 4) % 4);
        var bytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
        var claims = JsonDocument.Parse(bytes);
        godId = Guid.Parse(claims.RootElement.GetProperty("sub").GetString()!);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return (client, godId);
    }

    private async Task<ChampionDto> CreateChampionAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("api/champions",
            new { name = "TestHero", @class = 0 }); // 0 = Warrior
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ChampionDto>())!;
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task GetMyChampion_WithoutChampion_Returns404()
    {
        var (client, _) = await AuthenticatedClientAsync();

        var response = await client.GetAsync("api/champions/me");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateChampion_Returns201_WithChampionData()
    {
        var (client, godId) = await AuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("api/champions",
            new { name = "Aldric", @class = 0 });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var champion = await response.Content.ReadFromJsonAsync<ChampionDto>();
        Assert.NotNull(champion);
        Assert.Equal("Aldric", champion!.Name);
        Assert.Equal(godId, champion.GodId);
        Assert.Equal(1, champion.Level);
        Assert.Equal(100, champion.MaxHP);
    }

    [Fact]
    public async Task GetMyChampion_AfterCreation_Returns200()
    {
        var (client, _) = await AuthenticatedClientAsync();
        await CreateChampionAsync(client);

        var response = await client.GetAsync("api/champions/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var champion = await response.Content.ReadFromJsonAsync<ChampionDto>();
        Assert.NotNull(champion);
        Assert.Equal(1, champion!.Level);
    }

    [Fact]
    public async Task SubmitIntervention_Returns201()
    {
        var (client, _) = await AuthenticatedClientAsync();
        var champion = await CreateChampionAsync(client);

        var response = await client.PostAsJsonAsync("api/interventions",
            new { championId = champion.Id, rawCommand = "blessed blade" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var intervention = await response.Content.ReadFromJsonAsync<InterventionDto>();
        Assert.NotNull(intervention);
        Assert.Equal("blessed blade", intervention!.RawCommand);
        Assert.False(intervention.IsApplied);
    }

    [Fact]
    public async Task DevTick_UpdatesChampionXP_AndCreatesNarrative()
    {
        var (client, _) = await AuthenticatedClientAsync();
        var champion = await CreateChampionAsync(client);
        var xpBefore = champion.XP;

        // Submit an intervention first
        await client.PostAsJsonAsync("api/interventions",
            new { championId = champion.Id, rawCommand = "heal" });

        // Trigger the world tick
        var tickResponse = await client.PostAsync("api/dev/tick", null);
        Assert.Equal(HttpStatusCode.OK, tickResponse.StatusCode);

        // Champion XP should have increased
        var updatedResp = await client.GetAsync("api/champions/me");
        updatedResp.EnsureSuccessStatusCode();
        var updated = await updatedResp.Content.ReadFromJsonAsync<ChampionDto>();
        Assert.NotNull(updated);
        Assert.True(updated!.XP > xpBefore, $"Expected XP > {xpBefore} but got {updated.XP}");

        // Narrative entry should have been created
        var narrativesResp = await client.GetAsync("api/champions/me/narratives");
        narrativesResp.EnsureSuccessStatusCode();
        var narratives = await narrativesResp.Content.ReadFromJsonAsync<List<NarrativeDto>>();
        Assert.NotNull(narratives);
        Assert.NotEmpty(narratives!);
        Assert.Contains("The champion ventured forth", narratives[0].StoryText);
    }

    // ---------------------------------------------------------------------------
    // DTOs (match JSON shape returned by API)
    // ---------------------------------------------------------------------------

    private record ChampionDto(
        Guid Id, Guid GodId, string Name, int Class,
        int STR, int DEX, int INT, int WIS, int VIT,
        int HP, int MaxHP, int Level, int XP,
        string? PowerUpSlot, int Biome,
        DateTimeOffset CreatedAt, DateTimeOffset LastTickAt);

    private record InterventionDto(
        Guid Id, Guid GodId, Guid ChampionId,
        string RawCommand, bool IsApplied, DateTimeOffset CreatedAt);

    private record NarrativeDto(
        Guid Id, Guid ChampionId, int TickNumber, string StoryText, DateTimeOffset CreatedAt);
}
