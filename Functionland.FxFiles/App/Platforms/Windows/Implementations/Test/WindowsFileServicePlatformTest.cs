using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.App.Platforms.Windows.Implementations.Test;

public partial class WindowsFileServicePlatformTest : FileServicePlatformTest
{
    public IFileService FileService { get; set; }

    public WindowsFileServicePlatformTest(IFileService fileService)
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
