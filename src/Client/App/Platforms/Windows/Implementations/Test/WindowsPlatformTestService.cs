using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public partial class WindowsPlatformTestService : PlatformTestService
{
    // File Service
    [AutoInject] WindowsFileServicePlatformTest WindowsFileServicePlatformTest { get; set; }

    // Thumbnail Plugins
    [AutoInject] WindowsImageThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalWindowsImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] WindowsImageThumbnailPluginPlatformTest<IFulaFileService> FulaWindowsImageThumbnailPluginPlatformTest { get; set; }
    [AutoInject] WindowsPdfThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalWindowsPdfThumbnailPluginPlatformTest { get; set; }
    [AutoInject] WindowsPdfThumbnailPluginPlatformTest<IFulaFileService> FulaWindowsPdfThumbnailPluginPlatformTest { get; set; }
    [AutoInject] WindowsVideoThumbnailPluginPlatformTest<ILocalDeviceFileService> LocalWindowsVideoThumbnailPluginPlatformTest { get; set; }
    [AutoInject] WindowsVideoThumbnailPluginPlatformTest<IFulaFileService> FulaWindowsVideoThumbnailPluginPlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            WindowsFileServicePlatformTest,
            LocalWindowsImageThumbnailPluginPlatformTest,
            FulaWindowsImageThumbnailPluginPlatformTest,
            LocalWindowsPdfThumbnailPluginPlatformTest,
            FulaWindowsPdfThumbnailPluginPlatformTest,
            LocalWindowsVideoThumbnailPluginPlatformTest,
            FulaWindowsVideoThumbnailPluginPlatformTest
        };
    }  
}
