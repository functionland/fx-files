namespace Microsoft.Extensions.DependencyInjection;

public static class IiOSServiceCollectionExtensions
{
    public static IServiceCollection AddClientiOSServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in iOS.

        //TODO: services.AddSingleton<IFileService, IosFileService>();
        //services.AddTransient<IPlatformTestService, IosPlatformTestService>();
        //services.AddTransient<IosFileServicePlatformTest>();

        return services;
    }
}
