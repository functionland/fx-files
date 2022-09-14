using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.iOS.Implementations
{
    public partial class IosPlatformTestService : PlatformTestService
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
}
