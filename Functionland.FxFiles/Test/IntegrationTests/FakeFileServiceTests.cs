using Functionland.FxFiles.Shared.Services;
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
    public class FakeFileServiceTests : TestBase
    {
        [TestMethod]
        public async Task FakelatformTest_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.AddAppServices();
                   //services.AddSingleton<IFileService>(FakeFileServiceFactory.CreateSimpleFileListOnRoot());
               }
            ).Build();

            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            //var fileService = serviceProvider.GetService<IFileService>();

            //Assert.IsNotNull(fileService);

            //var list = await fileService.GetArtifactsAsync().ToListAsync();
            //Assert.AreEqual(3, list.Count);

            var testRunner = serviceProvider.GetService<IPlatformTestService>();
            Assert.IsNotNull(testRunner);
            
            try
            {
                testRunner.TestProgressChanged += Test_ProgressChanged;
                var tests = testRunner.GetTests();
                foreach (var test in tests)
                {
                    await testRunner.RunTestAsync(test);
                }
            }
            finally
            {
                testRunner.TestProgressChanged -= Test_ProgressChanged;
            }

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
