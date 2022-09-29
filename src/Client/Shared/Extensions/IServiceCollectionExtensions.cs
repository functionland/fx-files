using Functionland.FxFiles.Client.Shared.Services;
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

        services.AddSingleton<IPinService, PinService>();
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IThumbnailService, FakeThumbnailService>();
        services.AddSingleton<FakeFileServiceFactory>();
        return services;
    }

    public static async Task RunAppEvents(this IServiceProvider serviceProvider, AppEventOption? option = null)
    {
        var exceptionHandler = serviceProvider.GetRequiredService<IExceptionHandler>();
        try
        {
            var FxLocalDbService = serviceProvider.GetRequiredService<IFxLocalDbService>();
            var PinService = serviceProvider.GetRequiredService<IPinService>();

            await FxLocalDbService.InitAsync();
            await PinService.InitializeAsync();
        }
        catch (Exception ex)
        {
            exceptionHandler.Handle(ex);
        }
    }
}

public class AppEventOption
{
    //TODO: Put something that you need in your app events.
}
