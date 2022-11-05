using Functionland.FxFiles.Client.App.Platforms.Android.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IMacServiceCollectionExtensions
{
    public static IServiceCollection AddClientMacServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Mac.

        services.AddSingleton<IPathUtilService, MacPathUtilService>();

        return services;
    }
}
