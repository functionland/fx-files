using Functionland.FxFiles.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Shared.Test.Utils;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using Microsoft.Extensions.Hosting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Test.UnitTests
{
    [TestClass]
    public class PinServiceUnitTest : TestBase
    {
        [TestMethod]
        public async Task AddPinUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddAppServices();
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var pinService = serviceProvider.GetService<IPinService>();

            await pinService.SetArtifactsPinAsync(
                new FsArtifact[] {new FsArtifact("C:\\Program Files", "Program Files", FsArtifactType.Folder, FsFileProviderType.InternalMemory) });

            var pinnedFiles = pinService.GetPinnedArtifactsAsync(null);
            await foreach(var file in pinnedFiles)
            {
                Console.WriteLine(file.FullPath);
            }
            await pinService.SetArtifactsUnPinAsync(new string[] {"C:\\Program Files"});
            pinnedFiles = pinService.GetPinnedArtifactsAsync(null);
            await foreach (var file in pinnedFiles)
            {
                Console.WriteLine(file.FullPath);
            }

        }
       
        [TestMethod]
        public async Task PublishEvent_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddSingleton<IPinService, PinService>();
                   services.AddSingleton<IEventAggregator, EventAggregator>();
                   services.AddSingleton<IFileService, FakeFileService>();

                   string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FxDB.db")};";

                   services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));

               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var pinService = serviceProvider.GetService<IPinService>();
            var aggrigator = serviceProvider.GetService<IEventAggregator>();

            await pinService.InitializeAsync();
            aggrigator.GetEvent<ArtifactChangeEvent>().Publish(new ArtifactChangeEvent()
            {
                ChangeType = FsArtifactChangesType.Delete,
                FsArtifact = new FsArtifact("C:\\Program Files", "Program Files", FsArtifactType.Folder, FsFileProviderType.InternalMemory)
           
            });
        }


    }
}
