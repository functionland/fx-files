using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientWebServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in web (blazor web assembly & blzor server)
        string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FxDB.db")};";
        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));


        services.AddClientSharedServices();

        services.AddSingleton<IFileService>(
            (serviceProvider) =>
            serviceProvider.GetRequiredService<FakeFileServiceFactory>().CreateTypical()
            );
        services.AddSingleton<IFulaFileService>(
           (serviceProvider) =>
           serviceProvider.GetRequiredService<FakeFileServiceFactory>().CreateTypical()
           );
        services.AddSingleton<ILocalDeviceFileService>(
           (serviceProvider) =>
           serviceProvider.GetRequiredService<FakeFileServiceFactory>().CreateTypical()
           );

        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
        services.AddSingleton<IFileWatchService, FakeFileWatchService>();

        return services;
    }
}
