using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.X509Certificates;

namespace Functionland.FxFiles.Client.Test.UnitTests
{
    [TestClass]
    public class DbServiceUnitTest : TestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task AddPinDbServiceUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddClientSharedServices();
                   services.AddClientTestServices(TestContext);
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            var pinService = serviceProvider.GetService<ILocalDbPinService>();

            await pinService.AddPinAsync(new FsArtifact("c:\\txt.txt", "txt", FsArtifactType.File, FsFileProviderType.InternalMemory));
            //Assert.IsNotNull(fileService);

        }

        [TestMethod]
        public async Task RemovePinDbServiceUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FxDB.db")};";

                   services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
                   services.AddSingleton<ILocalDbPinService, LocalDbPinService>();
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            var pinService = serviceProvider.GetService<ILocalDbPinService>();
            await pinService.RemovePinAsync("c:\\txt.txt");
            //Assert.IsNotNull(fileService);

        }

        [TestMethod]
        public async Task UpdatePinDbServiceUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FxDB.db")};";

                   services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
                   services.AddSingleton<ILocalDbPinService, LocalDbPinService>();
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            var pinService = serviceProvider.GetService<ILocalDbPinService>();
            await pinService.UpdatePinAsync(new PinnedArtifact
            {
                FullPath = "c:\\txt.txt",
                ContentHash = DateTimeOffset.Now.AddDays(-1).ToString(),
                ThumbnailPath = "c:\\txt.txt"
            });
            //Assert.IsNotNull(fileService);

        }

        [TestMethod]
        public async Task GetPinnedArtifacsDbServiceUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FxDB.db")};";

                   services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
                   services.AddSingleton<ILocalDbPinService, LocalDbPinService>();
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            var pinService = serviceProvider.GetService<ILocalDbPinService>();
            var pinnedArtifacts = await pinService.GetPinnedArticatInfos();
            Assert.IsNotNull(pinnedArtifacts);

        }

        private void Test_ProgressChanged(object? sender, TestProgressChangedEventArgs e)
        {
            if (e.ProgressType == TestProgressType.Fail)
            {
                Assert.Fail($"{Environment.NewLine}{sender?.GetType().Name}{Environment.NewLine}-> {e.Title}{Environment.NewLine}-> {e.Description}");
            }
        }
    }
}
