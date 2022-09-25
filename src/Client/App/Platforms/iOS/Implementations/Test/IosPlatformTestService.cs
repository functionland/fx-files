
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations.Test;

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
