using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
using Prism.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientSharedServices(this IServiceCollection services)
    {
        services.AddLocalization();

        services.AddScoped<ThemeInterop>();

        services.AddAuthorizationCore();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton<IExceptionHandler, ExceptionHandler>();

        services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
        services.AddScoped(sp => (AppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

#if BlazorHybrid
        string connectionString = $"DataSource={Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "FxDB.db")};";
#else
        string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FxDB.db")};";
#endif

        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));

        services.AddSingleton<IPinService, PinService>();
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IThumbnailService, FakeThumbnailService>();

        return services;
    }
}
