using Functionland.FxFiles.Client.Shared.Services;
using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations.Test;

public partial class MacFileServicePlatformTest : FileServicePlatformTest
{
    public ILocalDeviceFileService FileService { get; set; }

    public MacFileServicePlatformTest(ILocalDeviceFileService fileService)
    {
        FileService = fileService;
    }

    public override string Title => "MacFileService Test";

    public override string Description => "Tests the common features of this FileService";

    protected override IFileService OnGetFileService()
    {
        return FileService;
    }

    protected override string OnGetTestsRootPath() => FileSystem.Current.CacheDirectory;
}
