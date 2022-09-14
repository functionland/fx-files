using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations.Test;

public partial class AndroidFileServicePlatformTest : FileServicePlatformTest
{
    public override string Title => "AndroidFileService Test";

    public override string Description => "Tests the common features of this FileService";

    protected override IFileService OnGetFileService()
    {
        return FakeFileServiceFactory.CreateTypical();
    }

    protected override string OnGetTestsRootPath() => "fakeroot";//TODO:Replace correct root path
}
