using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public partial class WindowsFileServicePlatformTest : FileServicePlatformTest
{
    public ILocalDeviceFileService FileService { get; set; }

    public WindowsFileServicePlatformTest(ILocalDeviceFileService fileService)
    {
        FileService = fileService;
    }
    public override string Title => "WindowsFileService Test";

    public override string Description => "Tests the common features of this FileService";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath() => "c:\\";
}
