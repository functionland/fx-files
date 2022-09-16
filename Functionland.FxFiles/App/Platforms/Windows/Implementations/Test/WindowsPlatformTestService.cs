using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations.Test;

public partial class WindowsPlatformTestService : PlatformTestService
{
    [AutoInject] WindowsFileServicePlatformTest WindowsFileServicePlatformTest { get; set; }
    
    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            WindowsFileServicePlatformTest
        };
    }  
}
