using Functionland.FxFiles.Client.Shared.Services;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations
{
    public partial class FakeFileServicePlatformTest_CreateSimpleFileListOnRoot : FileServicePlatformTest
    {
        [AutoInject] public IServiceProvider ServiceProvider { get; set; } = default!;
        public override string Title => "SimpleFileListOnRoot FakeFileService Test";

        public override string Description => "Tests the simple file list on root features of FakeFileService";

        protected override IFileService OnGetFileService()
        {
            return FakeFileServiceFactory.CreateSimpleFileListOnRoot(ServiceProvider, TimeSpan.Zero, TimeSpan.Zero);
        }

        protected override string OnGetTestsRootPath() => "fakeroot";
    }
}
