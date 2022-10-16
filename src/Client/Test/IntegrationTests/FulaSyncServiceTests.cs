using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionland.FxFiles.Client.Test.IntegrationTests
{
    [TestClass]
    public class FulaSyncServiceTests : TestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task SyncScenario01_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
                   services.AddSingleton<IFulaSyncService, FulaSyncService>();
                   services.AddSingleton<IFulaFileClient>(s => s.GetRequiredService<FakeFulaFileClientFactory>().CreateSyncScenario01());
                   services.AddSingleton<ILocalDbArtifactService>(s => s.GetRequiredService<FakeLocalDbArtifactServiceFactory>().CreateSyncScenario01());
                   services.AddSingleton<ILocalDbFulaSyncItemService, FakeLocalDbFulaSyncItemService>();
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var syncService = serviceProvider.GetService<IFulaSyncService>();
            var localDatabase = serviceProvider.GetService<ILocalDbArtifactService>();
            var fulaService = serviceProvider.GetService<IFulaFileClient>();

            var localRootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var localPath = Path.Combine(localRootPath, "MyFiles\\Music");
            var beforeSyncLocalDb = await localDatabase.GetArtifactAsync(localPath, "token,01");
            Assert.IsNull(beforeSyncLocalDb);

            var fulaRootPath = FulaConvention.FulaRootPath;
            var fulaPath = Path.Combine(fulaRootPath, "MyFiles\\Music");
            var beforeSyncFula = await fulaService.GetArtifactAsync("token,01", fulaPath);
            Assert.IsNotNull(beforeSyncFula);

            await syncService.InitAsync();

            var afterSyncLocalDb = await localDatabase.GetArtifactAsync(localPath, "token,01");
            Assert.IsNotNull(afterSyncLocalDb);

            var fulaAfterSync = await fulaService.GetArtifactAsync("token,01", fulaPath);
            Assert.IsNotNull(fulaAfterSync);
        }

        [TestMethod]
        public async Task SyncScenario02_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
                   services.AddSingleton<IFulaFileClient>(s => s.GetRequiredService<FakeFulaFileClientFactory>().CreateSyncScenario02());
                   services.AddSingleton<ILocalDbArtifactService>(s => s.GetRequiredService<FakeLocalDbArtifactServiceFactory>().CreateSyncScenario02());
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var syncService = serviceProvider.GetRequiredService<IFulaSyncService>();

            await syncService.SyncItemsAsync();

            // Assertions
        }
    }
}
