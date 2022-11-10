using Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;
using Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations.Test;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class IMacServiceCollectionExtensions
{
    public static IServiceCollection AddClientMacServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Mac.
        services.AddSingleton<IFulaFileService, FulaFileService>();
        services.AddSingleton<ILocalDeviceFileService, MacFileService>();

        services.AddSingleton<IPlatformTestService, MacPlatformTestService>();
        services.AddTransient<MacFileServicePlatformTest>();

        services.AddTransient<IThumbnailPlugin, MacImageThumbnailPlugin>();
        services.AddSingleton<IFileWatchService, MacFileWatchService>();

        services.AddSingleton<IFileCacheService, MacFileCacheService>();
        return services;
    }
}
