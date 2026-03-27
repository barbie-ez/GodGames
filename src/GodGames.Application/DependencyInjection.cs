using GodGames.Application.Interfaces;
using GodGames.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GodGames.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IGameEngineService, GameEngineService>();
        services.AddSingleton<IInterventionParser, InterventionParser>();

        return services;
    }
}
