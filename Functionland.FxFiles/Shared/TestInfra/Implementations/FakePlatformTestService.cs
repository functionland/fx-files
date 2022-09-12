namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public partial class FakePlatformTestService : PlatformTestService
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
