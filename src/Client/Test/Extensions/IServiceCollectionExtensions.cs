using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientTestServices(this IServiceCollection services, TestContext testContext)
    {
        string connectionString = $"DataSource={Path.Combine(testContext.TestDir, "FxDB.db")}";
        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));


        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
        services.AddSingleton<IFileService>(
            serviceProvider => serviceProvider.GetRequiredService<FakeFileServiceFactory>().CreateTypical()
        );
        services.AddSingleton<IFileWatchService, FakeFileWatchService>();
        return services;
    }
}
