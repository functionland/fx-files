
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public partial class AndroidPlatformTestService : PlatformTestService
{
    // File Service
    [AutoInject] InternalAndroidFileServicePlatformTest InternalAndroidFileServicePlatformTest { get; set; }
    [AutoInject] ExternalAndroidFileServicePlatformTest ExternalAndroidFileServicePlatformTest { get; set; }

    // Thumbnail Plugins
    [AutoInject] AndroidImageThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalAndroidImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidImageThumbnailPluginPlatformTest<IFulaFileService> FulaAndroidImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidPdfThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalAndroidPdfThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidPdfThumbnailPluginPlatformTest<IFulaFileService> FulaAndroidPdfThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidVideoThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalAndroidVideoThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidVideoThumbnailPluginPlatformTest<IFulaFileService> FulaAndroidVideoThumbnailPluginPlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            InternalAndroidFileServicePlatformTest,
            ExternalAndroidFileServicePlatformTest,
            LocalAndroidImageThumbnailPluginPlatformTest,
            FulaAndroidImageThumbnailPluginPlatformTest,
            LocalAndroidPdfThumbnailPluginPlatformTest,
            FulaAndroidPdfThumbnailPluginPlatformTest,
            LocalAndroidVideoThumbnailPluginPlatformTest,
            FulaAndroidVideoThumbnailPluginPlatformTest
        };
    }
}
