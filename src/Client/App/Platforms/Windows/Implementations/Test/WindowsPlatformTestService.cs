using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public partial class WindowsPlatformTestService : PlatformTestService
{
    [AutoInject] WindowsFileServicePlatformTest WindowsFileServicePlatformTest { get; set; }
    [AutoInject] ImageThumbnailPluginPlatformTest ImageThumbnailPluginPlatformTest { get; set; }
    
    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            WindowsFileServicePlatformTest,
            ImageThumbnailPluginPlatformTest
        };
    }  
}
