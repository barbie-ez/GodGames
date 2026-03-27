using GodGames.AI;

namespace GodGames.Tests.Integration;

/// <summary>Stub IAnthropicClient — returns a canned narrative without calling the Anthropic API.</summary>
public class StubAnthropicClient : IAnthropicClient
{
    public Task<string> CompleteAsync(string userMessage, int maxTokens, CancellationToken ct = default)
        => Task.FromResult("The champion ventured forth and emerged victorious. Glory awaits.");
}
