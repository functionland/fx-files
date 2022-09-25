using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientWebServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in web (blazor web assembly & blzor server)

        services.AddClientSharedServices();

        services.AddSingleton<IFileService>((serviceProvider) => FakeFileServiceFactory.CreateSimpleFileListOnRoot(serviceProvider));
        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
        services.AddSingleton<IFileWatchService, FakeFileWatchService>();

        return services;
    }
}
