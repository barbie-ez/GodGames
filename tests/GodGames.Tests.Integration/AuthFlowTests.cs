using System.Net;
using System.Net.Http.Json;

namespace GodGames.Tests.Integration;

[Collection("Api")]
public class AuthFlowTests(GodGamesApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidCredentials_Returns200()
    {
        var email = $"register-{Guid.NewGuid()}@test.com";
        var response = await _client.PostAsJsonAsync("api/auth/register",
            new { email, password = "Password1" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(body);
        Assert.Equal(email, body.Email);
        Assert.NotEqual(Guid.Empty, body.GodId);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        var email = $"dup-{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("api/auth/register", new { email, password = "Password1" });

        var response = await _client.PostAsJsonAsync("api/auth/register",
            new { email, password = "Password1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var email = $"login-{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("api/auth/register", new { email, password = "Password1" });

        var response = await _client.PostAsJsonAsync("api/auth/login",
            new { email, password = "Password1" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body?.Token);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var email = $"badpw-{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("api/auth/register", new { email, password = "Password1" });

        var response = await _client.PostAsJsonAsync("api/auth/login",
            new { email, password = "WrongPassword9" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private record RegisterResponse(Guid GodId, string Email);
    private record LoginResponse(string Token, DateTime ExpiresAt);
}
