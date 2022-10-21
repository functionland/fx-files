using Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;
using Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IWindowsServiceCollectionExtensions
{
    public static IServiceCollection AddClientWindowsServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Windows.
        services.AddSingleton<ILocalDeviceFileService, WindowsFileService>();

        //services.AddSingleton<IPlatformTestService, WindowsPlatformTestService>();
        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
        services.AddSingleton<IFileWatchService, FakeFileWatchService>();
        // services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<WindowsFileServicePlatformTest>();
        services.AddSingleton<IThumbnailService, WindowsThumbnailService>();
        services.AddSingleton<IFileWatchService, WindowsFileWatchService>();

        return services;
    }
}
