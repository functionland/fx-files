using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsImageThumbnailPluginPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    public WindowsImageThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                   TFileService fileService) : base(artifactThumbnailService, fileService)
    {
    }

    public override string Title => $"WindowsImageThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create artifact thumbnail on windows";

    protected override string OnGetRootPath() => "c:\\";
}
