
using Functionland.FxFiles.Client.Shared.Services;
using Prism.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientSharedServices(this IServiceCollection services)
    {
        services.AddLocalization();

        services.AddScoped<ThemeInterop>();
        services.AddScoped<ArtifactState>();

        services.AddAuthorizationCore();

        services.AddSingleton<IExceptionHandler, ExceptionHandler>();

        services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
        services.AddScoped(sp => (AppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddSingleton<IFulaFileService, FulaFileService>();
        services.AddSingleton<ILocalDevicePinService, LocalDevicePinService>();
        services.AddSingleton<IFulaPinService, FulaPinService>();

        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<IThumbnailService, FakeThumbnailService>();
        services.AddSingleton<FakeFileServiceFactory>();
        services.AddSingleton<FakeBloxServiceFactory>();
        services.AddSingleton<IBloxService, FakeBloxService>();
        services.AddSingleton<IGoBackService, GoBackService>();
        return services;
    }

    public static async Task RunAppEvents(this IServiceProvider serviceProvider, AppEventOption? option = null)
    {
        var exceptionHandler = serviceProvider.GetRequiredService<IExceptionHandler>();
        try
        {
            var FxLocalDbService = serviceProvider.GetRequiredService<IFxLocalDbService>();
            var PinService = serviceProvider.GetRequiredService<ILocalDevicePinService>();

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
