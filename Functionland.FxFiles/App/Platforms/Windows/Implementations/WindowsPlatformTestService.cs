using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations;

public partial class WindowsPlatformTestService : PlatformTestService
{
    [AutoInject] FakeFileServicePlatformTest FakeFileServicePlatformTest { get; set; }
    
    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            FakeFileServicePlatformTest
        };
    }  
}
