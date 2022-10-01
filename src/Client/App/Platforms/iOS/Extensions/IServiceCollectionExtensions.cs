using Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IiOSServiceCollectionExtensions
{
    public static IServiceCollection AddClientiOSServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in iOS.

        services.AddSingleton<ILocalDeviceFileService, IosFileService>();
        services.AddSingleton<IFulaFileService, IosFulaFileService>();

        services.AddSingleton<IFulaFileService, IosFulaFileService>();
        services.AddSingleton<ILocalDeviceFileService, IosFileService>();

        //services.AddTransient<IPlatformTestService, IosPlatformTestService>();
        //services.AddTransient<IosFileServicePlatformTest>();

        return services;
    }
}
