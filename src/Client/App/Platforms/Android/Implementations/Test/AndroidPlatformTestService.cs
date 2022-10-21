
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public partial class AndroidPlatformTestService : PlatformTestService
{
    [AutoInject] InternalAndroidFileServicePlatformTest InternalAndroidFileServicePlatformTest { get; set; }
    [AutoInject] ExternalAndroidFileServicePlatformTest ExternalAndroidFileServicePlatformTest { get; set; }
    [AutoInject] AndroidExternalArtifactThumbnailPlatformTest<ILocalDeviceFileService> LocalAndroidExternalImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidInternalArtifactThumbnailPlatformTest<ILocalDeviceFileService> LocalAndroidInternalImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidExternalArtifactThumbnailPlatformTest<IFulaFileService> FulaAndroidExternalImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] AndroidInternalArtifactThumbnailPlatformTest<IFulaFileService> FulaAndroidInternalImageThumbnailPluginPlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            InternalAndroidFileServicePlatformTest,
            ExternalAndroidFileServicePlatformTest,
            LocalAndroidExternalImageThumbnailPluginPlatformTest,
            LocalAndroidInternalImageThumbnailPluginPlatformTest,
            FulaAndroidExternalImageThumbnailPluginPlatformTest,
            FulaAndroidInternalImageThumbnailPluginPlatformTest,
        };
    }
}
