using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Android.Implementations.Test;

public partial class AndroidFileServicePlatformTest : FileServicePlatformTest
{
    public IFileService FileService { get; set; }

    public AndroidFileServicePlatformTest(IFileService fileService)
    {
        FileService = fileService;
    }
    public override string Title => "AndroidFileService Test";

    public override string Description => "Tests the common features of this FileService";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath() => "/storage/emulated/0/";//TODO:Replace correct root path
}
