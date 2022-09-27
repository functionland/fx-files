using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Shared.Services.Implementations.Db;
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

            await dbService.AddPinAsync(new FsArtifact("c:\\txt.txt", "txt", FsArtifactType.File, FsFileProviderType.InternalMemory));
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
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            await dbService.RemovePinAsync("c:\\txt.txt");
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
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            await dbService.UpdatePinAsync(new PinnedArtifact
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
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();
            await dbService.InitAsync();

            var pinnedArtifacts = await dbService.GetPinnedArticatInfos();
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
