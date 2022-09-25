using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations
{
    public partial class FakePlatformTestService : PlatformTestService
    {
        [AutoInject] FakeFileServicePlatformTest_CreateTypical FakeFileServicePlatformTest_CreateTypical { get; set; }
        [AutoInject] FakeFileServicePlatformTest_CreateSimpleFileListOnRoot FakeFileServicePlatformTest_CreateSimpleFileListOnRoot { get; set; }

        protected override List<IPlatformTest> OnGetTests()
        {
            return new List<IPlatformTest>()
            {
                FakeFileServicePlatformTest_CreateTypical,
                FakeFileServicePlatformTest_CreateSimpleFileListOnRoot
            };
        }
    }
}
