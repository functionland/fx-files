using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
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
                   services.AddSingleton<IFulaFileClient>(s => s.GetRequiredService<FakeFulaFileClientFactory>().CreateSyncScenario01());
                   services.AddSingleton<ILocalDbArtifactService>(s => s.GetRequiredService<FakeLocalDbArtifactServiceFactory>().CreateSyncScenario01());
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var syncService = serviceProvider.GetRequiredService<IFulaSyncService>();

            await syncService.SyncItemsAsync();

            // Assertions
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
