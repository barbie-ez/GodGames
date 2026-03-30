using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace GodGames.Web.Auth;

public class GodAuthStateProvider(IJSRuntime js) : AuthenticationStateProvider
{
    private const string TokenKey = "god_jwt";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        var principal = ParseToken(token);
        return new AuthenticationState(principal);
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        var principal = ParseToken(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task ClearTokenAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static ClaimsPrincipal ParseToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return Anonymous.User;

            var payload = parts[1];
            // Fix base64 padding and URL-safe chars
            payload = payload.Replace('-', '+').Replace('_', '/');
            while (payload.Length % 4 != 0) payload += "=";

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);

            var claims = new List<Claim>();
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                // Map JWT standard claims to .NET claim types
                var type = prop.Name switch
                {
                    "sub" => ClaimTypes.NameIdentifier,
                    "email" => ClaimTypes.Email,
                    _ => prop.Name
                };
                claims.Add(new Claim(type, prop.Value.ToString()));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
        catch
        {
            return Anonymous.User;
        }
    }
}
