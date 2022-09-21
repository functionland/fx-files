using Functionland.FxFiles.Shared.Services;

namespace Functionland.FxFiles.Shared.TestInfra.Implementations
{
    public partial class FakeFileServicePlatformTest_CreateTypical : FileServicePlatformTest
    {
        [AutoInject] public IServiceProvider ServiceProvider { get; set; } = default!;
        public override string Title => "FakeFileService Test";

        public override string Description => "Tests the common features of this FileService";

        protected override IFileService OnGetFileService()
        {
            return FakeFileServiceFactory.CreateTypical(ServiceProvider,TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override string OnGetTestsRootPath() => "fakeroot";
    }
}
