using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsImageThumbnailPluginPlatformTest<TFileService> : ImageThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    TFileService FileService { get; set; }

    public WindowsImageThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"WindowsImageThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create artifact thumbnail on windows";

    protected override string OnGetRootPath() => "c:\\";
}
