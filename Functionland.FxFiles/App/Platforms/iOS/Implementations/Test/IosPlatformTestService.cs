
namespace Functionland.FxFiles.App.Platforms.iOS.Implementations.Test;

public partial class IosPlatformTestService : PlatformTestService
{
    [AutoInject] IosFileServicePlatformTest IosFileServicePlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            IosFileServicePlatformTest
        };
    }
}
