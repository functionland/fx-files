
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientAppServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Android, iOS, Windows, and Mac.

        // Shared between all platforms
        string connectionString = $"DataSource={Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "FxDB.db")};";
        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
        services.AddSingleton<ILocalDbPinnedService, LocalDbPinnedService>();
        services.AddSingleton<ILocalDbArtifactService, LocalDbArtifactService>();
        services.AddSingleton<ILocalDbFulaSyncItemService, LocalDbFulaSyncItemService>();


#if ANDROID
        services.AddClientAndroidServices();
#elif iOS
        services.AddClientiOSServices();
#elif Mac
        services.AddClientMacServices();
#elif Windows
        services.AddClientWindowsServices();
#endif

        return services;
    }
}
