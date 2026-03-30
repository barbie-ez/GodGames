using GodGames.Mobile.Services;
using GodGames.Mobile.ViewModels;
using GodGames.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace GodGames.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // HTTP client — point at your API
        // NOTE: AddHttpClient<T> already registers the typed service — do NOT add AddSingleton<T> after it.
        // Android emulator cannot reach host localhost — use 10.0.2.2 instead.
#if ANDROID
        const string apiBase = "http://10.0.2.2:5134/";
#else
        const string apiBase = "http://localhost:5134/";
#endif

#if DEBUG
        var devHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(apiBase))
            .ConfigurePrimaryHttpMessageHandler(() => devHandler);
        builder.Services.AddHttpClient<GodGamesApiClient>(c => c.BaseAddress = new Uri(apiBase))
            .ConfigurePrimaryHttpMessageHandler(() => devHandler);
#else
        builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(apiBase));
        builder.Services.AddHttpClient<GodGamesApiClient>(c => c.BaseAddress = new Uri(apiBase));
#endif

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<CreateChampionViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ChroniclesViewModel>();
        builder.Services.AddTransient<WorldMapViewModel>();
        builder.Services.AddTransient<LeaderboardViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CreateChampionPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ChroniclesPage>();
        builder.Services.AddTransient<WorldMapPage>();
        builder.Services.AddTransient<LeaderboardPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
