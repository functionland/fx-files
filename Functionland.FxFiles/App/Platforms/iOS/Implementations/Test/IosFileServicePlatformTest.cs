using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.iOS.Implementations.Test;

public partial class IosFileServicePlatformTest : FileServicePlatformTest
{
    public override string Title => "IosFileService Test";

    public override string Description => "Tests the common features of this FileService";

    protected override IFileService OnGetFileService()
    {
        return FakeFileServiceFactory.CreateTypical();
    }

    protected override string OnGetTestsRootPath() => "fakeroot";
}
