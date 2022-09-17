using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Shared.Test.Utils;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionland.FxFiles.Shared.Test.UnitTests
{
    [TestClass]
    public class DbServiceUnitTest : TestBase
    {
        [TestMethod]
        public async Task AddPinDbServiceUnitTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FxDB.db")};";

                   services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var dbService = serviceProvider.GetService<IFxLocalDbService>();

            await dbService.AddPinAsync(new FsArtifact("c:\\txt.txt", "txt",FsArtifactType.File, FsFileProviderType.InternalMemory));
            //Assert.IsNotNull(fileService);

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
