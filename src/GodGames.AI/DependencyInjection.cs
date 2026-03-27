using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodGames.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAI(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["Anthropic:ApiKey"] ?? string.Empty;

        services.AddHttpClient<IAnthropicClient, AnthropicClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.anthropic.com/v1/");
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
