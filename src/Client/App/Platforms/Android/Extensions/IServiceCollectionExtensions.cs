using Functionland.FxFiles.Client.App.Platforms.Android.Implementations;
using Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class IAndroidServiceCollectionExtensions
{
    public static IServiceCollection AddClientAndroidServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Android.
        services.AddSingleton<ILocalDeviceFileService, AndroidFileService>();
        services.AddSingleton<IPlatformTestService, AndroidPlatformTestService>();
        services.AddTransient<InternalAndroidFileServicePlatformTest>();
        services.AddTransient<ExternalAndroidFileServicePlatformTest>();
        services.AddTransient<AndroidInternalArtifactThumbnailPlatformTest<ILocalDeviceFileService>>();
        services.AddTransient<AndroidExternalArtifactThumbnailPlatformTest<ILocalDeviceFileService>>();
        services.AddTransient<AndroidInternalArtifactThumbnailPlatformTest<IFulaFileService>>();
        services.AddTransient<AndroidExternalArtifactThumbnailPlatformTest<IFulaFileService>>();
        services.AddSingleton<IFileWatchService, AndroidFileWatchService>();
        services.AddSingleton<IFileCacheService, AndroidFileCacheService>();
        services.AddSingleton<IThumbnailPlugin, AndroidImageThumbnailPlugin>();

        return services;
    }
}
