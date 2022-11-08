using Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;
using Functionland.FxFiles.Client.App.Platforms.iOS.Implementations.Test;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;


namespace Microsoft.Extensions.DependencyInjection;

public static class IiOSServiceCollectionExtensions
{
    public static IServiceCollection AddClientiOSServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in iOS.

        services.AddSingleton<IFulaFileService, FulaFileService>();
        services.AddSingleton<ILocalDeviceFileService, IosFileService>();

        services.AddSingleton<IPlatformTestService, IosPlatformTestService>();
        services.AddTransient<IosFileServicePlatformTest>();

        services.AddSingleton<IFileWatchService, IosFileWatchService>();

        services.AddSingleton<IFileCacheService, IosFileCacheService>();

        services.AddSingleton<IZipPathUtilService, IosZipPathUtilService>();

        return services;
    }
}
