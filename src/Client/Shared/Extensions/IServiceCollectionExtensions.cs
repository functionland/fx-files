using Functionland.FxFiles.Client.Shared.Services;
using Prism.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientSharedServices(this IServiceCollection services)
    {
        services.AddLocalization();

        services.AddScoped<ThemeInterop>();
        services.AddSingleton<InMemoryAppStateStore>();

        services.AddAuthorizationCore();

        services.AddSingleton<IExceptionHandler, ExceptionHandler>();

        services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
        services.AddScoped(sp => (AppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddSingleton<IFulaFileService, FulaFileService>();
        services.AddSingleton<ILocalDevicePinService, LocalDevicePinService>();
        services.AddSingleton<IFulaPinService, FulaPinService>();

        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<FakeFileServiceFactory>();
        services.AddSingleton<FakeBloxServiceFactory>();
        services.AddSingleton<FakePoolServiceFactory>();
        services.AddSingleton<IBloxService, FakeBloxService>();
        services.AddSingleton<IFulaPoolSevice, FakePoolService>();
        services.AddSingleton<IGoBackService, GoBackService>();
        services.AddSingleton<IZipService, ZipService>();

        services.AddTransient<IArtifactThumbnailService<ILocalDeviceFileService>, ArtifactThumbnailService<ILocalDeviceFileService>>();
        services.AddTransient<IArtifactThumbnailService<IFulaFileService>, ArtifactThumbnailService<IFulaFileService>>();


        services.AddSingleton<FakeFulaFileClientFactory>();
        services.AddSingleton<ILocalDbArtifactService, FakeLocalDbArtifactService>();
        services.AddSingleton<FakeLocalDbArtifactServiceFactory>();

        services.AddSingleton<FsFileProviderDependency>();

        services.AddSingleton<IntentHolder>();

        return services;
    }

    public static async Task RunAppEvents(this IServiceProvider serviceProvider)
    {
        var exceptionHandler = serviceProvider.GetRequiredService<IExceptionHandler>();
        try
        {
            var FxLocalDbService = serviceProvider.GetRequiredService<IFxLocalDbService>();
            var PinService = serviceProvider.GetRequiredService<ILocalDevicePinService>();
            var FileCacheService = serviceProvider.GetRequiredService<IFileCacheService>();

            await FxLocalDbService.InitAsync();
            var pinTask = PinService.InitializeAsync();
            var cacheTask = FileCacheService.InitAsync();

            await Task.WhenAll(pinTask, cacheTask);
        }
        catch (Exception ex)
        {
            exceptionHandler.Handle(ex);
        }
    }
}
