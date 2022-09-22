using Functionland.FxFiles.Shared.Services;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public partial class FakeFileServicePlatformTest_CreateTypical : FileServicePlatformTest
    {
        [AutoInject] public IServiceProvider ServiceProvider { get; set; } = default!;
        public override string Title => "Typical FakeFileService Test";

        public override string Description => "Tests the typical features of this FakeFileService";

        protected override IFileService OnGetFileService()
        {
            return FakeFileServiceFactory.CreateTypical(ServiceProvider,TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override string OnGetTestsRootPath() => "fakeroot";
    }
}
