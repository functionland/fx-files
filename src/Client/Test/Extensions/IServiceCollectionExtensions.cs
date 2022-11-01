using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System.Reflection;
using Functionland.FxFiles.Client.Shared.Services.Implementations.IdentityService;
using Functionland.FxFiles.Client.Test.Services.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientTestServices(this IServiceCollection services, TestContext testContext)
    {
        string connectionString = $"DataSource={Path.Combine(testContext.TestDir, "FxDB.db")}";
        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));


        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddSingleton<ILocalDevicePinService, LocalDevicePinService>();

        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
        services.AddSingleton<ILocalDeviceFileService>(
            serviceProvider => serviceProvider.GetRequiredService<FakeFileServiceFactory>().CreateTypical()
        );
        services.AddSingleton<IFileWatchService, FakeFileWatchService>();

        var cacheDirectory = Path.Combine(testContext.TestDir, "TestCache");
        Directory.CreateDirectory(cacheDirectory);
        services.AddSingleton<IFileCacheService>(new FakeFileCacheService(cacheDirectory));

        return services;
    }
}
