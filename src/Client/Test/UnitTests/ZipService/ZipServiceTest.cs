using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System.Text;
using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Test.Services.Implementations;

namespace Functionland.FxFiles.Client.Test.UnitTests
{
    [TestClass]
    public class ZipServiceTest : TestBase
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task OpenSimpleZip_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("SimpleZip.zip"));
            Assert.AreEqual(3, artifacts.Count);
        }

        [TestMethod]
        public async Task OpenSimpleZipWithUppercaseExtension_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("SimpleZipWithUppercaseExtension.ZIP"));
            Assert.AreEqual(3, artifacts.Count);

        }

        [TestMethod]
        public async Task OpenNonExistedFile_ShouldThrowFileNotFound()
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

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () =>
            {
                var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("NOFILE.zip"));
            });
        }

        [TestMethod]
        public async Task OpenProtectedZip_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ProtectedWith123Zip.zip"));
            Assert.AreEqual(9, artifacts.Count);
        }

        [TestMethod]
        public async Task OpenProtectedRar_MustWork()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ProtectedWith123Rar.rar"));
            Assert.AreEqual(9, artifacts.Count);
        }

        [TestMethod]
        public async Task OpenProtectedEncryptedRar_MustThrowException()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();

            
            await Assert.ThrowsExceptionAsync<InvalidPasswordException>(async () =>
            {
                var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ProtectedWith123EncryptedEnabledRar.rar"));
            });
        }

        [TestMethod]
        public async Task OpenProtectedEncryptedRarWithCorrectPassword_MustThrowException()
        {
            var testHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        services.AddClientSharedServices();
                        services.AddClientTestServices(TestContext);
                        services.AddTransient<ILocalDeviceFileService, GenericFileService>();
                    }
                ).Build();


            var serviceScope = testHost.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var zipService = serviceProvider.GetRequiredService<IZipService>();


            await Assert.ThrowsExceptionAsync<NotSupportedEncryptedFileException>(async () =>
            {
                var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ProtectedWith123EncryptedEnabledRar.rar"), "123");
            });
        }

        private string GetSamplePath(string filename)
        {
            return Path.Combine(TestContext.DeploymentDirectory, "UnitTests", "ZipService", "SampleArchives", filename);
        }
    }
}