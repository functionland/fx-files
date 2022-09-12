using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations
{
    public partial class AndroidPlatformTestService : PlatformTestService
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
