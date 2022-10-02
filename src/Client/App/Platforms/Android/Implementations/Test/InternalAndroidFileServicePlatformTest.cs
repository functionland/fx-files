using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public partial class InternalAndroidFileServicePlatformTest : FileServicePlatformTest
{
    public ILocalDeviceFileService FileService { get; set; }

    public InternalAndroidFileServicePlatformTest(ILocalDeviceFileService fileService)
    {
        FileService = fileService;
    }
    public override string Title => "Internal AndroidFileService Test";

    public override string Description => "Tests the common features of this FileService internal storage of Android.";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath() => "/storage/emulated/0/";//TODO:Replace correct root path
}
