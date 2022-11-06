
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations.Test;

public partial class MacPlatformTestService : PlatformTestService
{
    [AutoInject] MacFileServicePlatformTest MacFileServicePlatformTest { get; set; }

    protected override List<IPlatformTest> OnGetTests()
    {
        return new List<IPlatformTest>()
        {
            MacFileServicePlatformTest
        };
    }
}
