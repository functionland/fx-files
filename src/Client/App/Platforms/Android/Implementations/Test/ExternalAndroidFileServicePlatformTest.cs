using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public partial class ExternalAndroidFileServicePlatformTest : FileServicePlatformTest
{
    public ILocalDeviceFileService FileService { get; set; }

    public ExternalAndroidFileServicePlatformTest(ILocalDeviceFileService fileService)
    {
        FileService = fileService;
    }
    public override string Title => "External AndroidFileService Test";

    public override string Description => "Tests the common features of this FileService external storage of Android.";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath() => "/emulated/0/";//TODO:Replace correct root path
}
