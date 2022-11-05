using Functionland.FxFiles.Client.App.Implementations;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientAppServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Android, iOS, Windows, and Mac.

        // Shared between all platforms
        string connectionString = $"DataSource={Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "FxDB.db")};";
        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
        services.AddSingleton<INativeNavigation, NativeNavigation>();

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
