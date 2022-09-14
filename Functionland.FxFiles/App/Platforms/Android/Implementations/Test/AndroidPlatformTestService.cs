
namespace Functionland.FxFiles.App.Platforms.Android.Implementations.Test;

public partial class AndroidPlatformTestService : PlatformTestService
{
    [AutoInject] AndroidFileServicePlatformTest AndroidFileServicePlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            AndroidFileServicePlatformTest
        };
    }
}
