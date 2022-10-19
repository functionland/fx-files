using Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;
using Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class IWindowsServiceCollectionExtensions
{
    public static IServiceCollection AddClientWindowsServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Windows.
        services.AddSingleton<ILocalDeviceFileService, WindowsFileService>();
        services.AddSingleton<IPlatformTestService, WindowsPlatformTestService>();
        services.AddTransient<WindowsFileServicePlatformTest>();
        services.AddTransient<WindowsImageThumbnailPluginPlatformTest<ILocalDeviceFileService>>();
        services.AddTransient<WindowsImageThumbnailPluginPlatformTest<IFulaFileService>>();
        services.AddSingleton<IFileWatchService, WindowsFileWatchService>();
        services.AddSingleton<IFileCacheService, WindowsFileCacheService>();

        services.AddTransient<IThumbnailPlugin, WindowsImageThumbnailPlugin>();

        return services;
    }
}
