using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace GodGames.AI;

public interface IAnthropicClient
{
    Task<string> CompleteAsync(string userMessage, int maxTokens, CancellationToken ct = default);
}

public class AnthropicClient(
    HttpClient httpClient,
    IOptions<AnthropicOptions> options) : IAnthropicClient
{
    private static readonly string SystemPrompt =
        "You are a narrator for a fantasy idle RPG. Write exactly 2 vivid sentences describing what happened to the champion. Be concise and dramatic.";

    public async Task<string> CompleteAsync(string userMessage, int maxTokens, CancellationToken ct = default)
    {
        var model = options.Value.Model;

        var requestBody = new
        {
            model,
            max_tokens = maxTokens,
            system = SystemPrompt,
            messages = new[]
            {
                new { role = "user", content = userMessage }
            }
        };

        var response = await httpClient.PostAsJsonAsync("messages", requestBody, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);

        var text = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        return text ?? throw new InvalidOperationException("Anthropic returned empty content.");
    }
}
