using System.Text;
using GodGames.AI;
using GodGames.API.Services;
using GodGames.Application.Interfaces;
using GodGames.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Testcontainers.PostgreSql;

namespace GodGames.Tests.Integration;

public class GodGamesApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        // Apply EF migrations against the fresh container
        var options = new DbContextOptionsBuilder<GodGamesDbContext>()
            .UseNpgsql(_db.GetConnectionString())
            .Options;
        await using var ctx = new GodGamesDbContext(options);
        await ctx.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _db.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _db.GetConnectionString(),
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["Anthropic:ApiKey"] = "test-key",
                ["Anthropic:Model"] = "claude-sonnet-4-20250514",
                ["Jwt:Key"] = "integration-test-secret-key-32-bytes!!",
                ["Jwt:Issuer"] = "GodGames",
                ["Jwt:Audience"] = "GodGames",
                // Silence file logging in tests
                ["Serilog:WriteTo:1:Name"] = "Console",
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove ChampionUpdateListener — it injects IConnectionMultiplexer and starts on boot
            var listenerDescriptor = services.SingleOrDefault(
                d => d.ImplementationType == typeof(ChampionUpdateListener));
            if (listenerDescriptor != null) services.Remove(listenerDescriptor);

            // Remove IConnectionMultiplexer so Redis is never contacted
            var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null) services.Remove(redisDescriptor);

            // Replace ITickNotifier with a no-op (avoids Redis dependency)
            var notifierDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITickNotifier));
            if (notifierDescriptor != null) services.Remove(notifierDescriptor);
            services.AddSingleton<ITickNotifier, NoOpTickNotifier>();

            // Replace IAnthropicClient with a stub (avoids real API calls)
            var anthropicDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAnthropicClient));
            if (anthropicDescriptor != null) services.Remove(anthropicDescriptor);
            services.AddSingleton<IAnthropicClient, StubAnthropicClient>();

            // Fix JWT validation: Program.cs captures the key before WebApplicationFactory
            // overrides take effect (early binding). PostConfigure runs after Build() with
            // our in-memory config, so IConfiguration has the test key here.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var testKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("integration-test-secret-key-32-bytes!!"));
                options.TokenValidationParameters.IssuerSigningKey = testKey;
                options.TokenValidationParameters.ValidIssuer = "GodGames";
                options.TokenValidationParameters.ValidAudience = "GodGames";
            });
        });
    }
}
