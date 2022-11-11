using Functionland.FxFiles.Client.Shared.Exceptions;
using Functionland.FxFiles.Client.Shared.Services.Contracts;
using Functionland.FxFiles.Client.Test.Services.Implementations;

using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Functionland.FxFiles.Client.Test.UnitTests
{
    [TestClass]
    public class ZipServiceTest : TestBase
    {
        public TestContext TestContext { get; set; } = default!;

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
        public async Task OpenZipFileWithUppercaseExtension_MustWork()
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

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ZipFileWithUppercaseExtension.ZIP"));
            Assert.AreEqual(9, artifacts.Count);

        }

        [TestMethod]
        public async Task ExtractZipFileWithUppercaseExtension_MustWork()
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

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("ZipFileWithUppercaseExtension.ZIP"),
                                                        GetSampleDestinationPath(),
                                                        "ZipFileWithUppercaseExtension");
        }

        [Ignore]
        public async Task OpenRarFileWithUppercaseExtension_MustWork()
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

            var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("RarFileWithUppercaseExtension.RAR"));
            Assert.AreEqual(9, artifacts.Count);
        }

        [Ignore]
        public async Task ExtractRarFileWithUppercaseExtension_MustWork()
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
            var x = GetSamplePath("RarFileWithUppercaseExtension.RAR");
            await zipService.ExtractZippedArtifactAsync(GetSamplePath("RarFileWithUppercaseExtension.RAR"),
                                                        GetSampleDestinationPath(),
                                                        "RarFileWithUppercaseExtension");
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
                await zipService.GetAllArtifactsAsync(GetSamplePath("NOFILE.zip"));
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
        public async Task ContentZipFile_MustWork()
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
            const string itemPath = "Folder 1/b.txt";
            var artifact = artifacts.Where(a => a.FullPath == itemPath).ToList();

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("SimpleZip.zip"),
                                                        GetSampleDestinationPath(),
                                                        "ExtractZipFile",
                                                        artifact);

            var filePath = Path.Combine(GetSampleDestinationPath(), "ExtractZipFile", Path.GetFileName(itemPath));
            var allText = await File.ReadAllTextAsync(filePath);

            var checkTextInsideFile = allText.Equals("Test extract zip file\r\n");
            Assert.IsTrue(checkTextInsideFile);
        }


        [Ignore]
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

        [Ignore]
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


            await Assert.ThrowsExceptionAsync<NotSupportedEncryptedFileException>(async () =>
            {
                var artifacts = await zipService.GetAllArtifactsAsync(GetSamplePath("ProtectedWith123EncryptedEnabledRar.rar"));
            });
        }

        [Ignore]
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

        [TestMethod]
        public async Task ExtractZipFile_MustWork()
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

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("SimpleZip.zip"), GetSampleDestinationPath(), "ExtractZipFile");

        }

        [Ignore]
        public async Task ExtractRarFile_MustWork()
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

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("SimpleRar.rar"), GetSampleDestinationPath(), "ExtractRarFile");
        }

        [TestMethod]
        public async Task ExtractZipFileWithCorrectPassword_MustWork()
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

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Zip.zip"),
                                                        GetSampleDestinationPath(),
                                                        "ZipFileWithCorrectPassword",
                                                        null,
                                                        false,
                                                        "123");
        }

        [Ignore]
        public async Task ExtractRarFileWithCorrectPassword_MustWork()
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

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Rar.rar"),
                                                        GetSampleDestinationPath(),
                                                        "RarFileWithCorrectPassword",
                                                        null,
                                                        false,
                                                        "123");
        }

        [TestMethod]
        public async Task ExtractInnerArtifactInZipFile_MustWork()
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
            var artifact = artifacts.Where(a => a.FullPath == "Folder 1/b.txt").ToList();

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("SimpleZip.zip"),
                                                        GetSampleDestinationPath(),
                                                        "ExtractOneArtifactInZipFile",
                                                        artifact);
        }

        [TestMethod]
        public async Task ExtractInnerArtifactInZipFileWithCorrectPassword_MustWork()
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

            var artifact = artifacts.Where(a => a.FullPath.StartsWith("A/b/f.txt")).ToList();

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Zip.zip"),
                                                        GetSampleDestinationPath(),
                                                        "ZipFileWithCorrectPassword",
                                                        artifact,
                                                        false,
                                                        "123");
        }

        [TestMethod]
        public async Task ExtractInnerArtifactInZipFileWithIncorrectPassword_MustThrowException()
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

            var artifact = artifacts.Where(a => a.FullPath.StartsWith("A/b/f.txt")).ToList();

            await Assert.ThrowsExceptionAsync<InvalidPasswordException>(async () =>
            {
                await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Zip.zip"),
                                                            GetSampleDestinationPath(),
                                                            "ZipFileWithIncorrectPassword",
                                                            artifact,
                                                            false,
                                                            "1855");
            });

        }

        [TestMethod]
        public async Task ExtractInnerArtifactInZipFileNotEnteredPassword_MustThrowException()
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

            var artifact = artifacts.Where(a => a.FullPath.StartsWith("A/b/f.txt")).ToList();

            await Assert.ThrowsExceptionAsync<InvalidPasswordException>(async () =>
            {
                await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Zip.zip"),
                                                            GetSampleDestinationPath(),
                                                            "ZipFileNotEnteredPassword",
                                                            artifact,
                                                            false);
            });
        }

        [Ignore]
        public async Task ExtractInnerArtifactInRarFileWithCorrectPassword_MustWork()
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

            var artifact = artifacts.Where(a => a.FullPath.StartsWith("A/b/f.txt")).ToList();

            await zipService.ExtractZippedArtifactAsync(GetSamplePath("ProtectedWith123Rar.rar"),
                                                        GetSampleDestinationPath(),
                                                        "RarFileWithCorrectPassword",
                                                        artifact,
                                                        false,
                                                        "123");
        }

        private string GetSamplePath(string filename)
        {
            return Path.Combine(TestContext.DeploymentDirectory, "UnitTests", "ZipService", "SampleArchives", filename);
        }
        private string GetSampleDestinationPath()
        {
            return Path.Combine(TestContext.DeploymentDirectory, "UnitTests", "ZipService", "SampleArchives");
        }
    }
}