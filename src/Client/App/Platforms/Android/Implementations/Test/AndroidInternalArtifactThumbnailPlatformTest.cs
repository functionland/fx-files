using Functionland.FxFiles.Client.Shared.TestInfra.Implementations;

namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;

public class AndroidInternalArtifactThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    public AndroidInternalArtifactThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService,
                                                           TFileService fileService) : base(artifactThumbnailService, fileService)
    {
    }

    public override string Title => $"AndroidInternalArtifactThumbnailPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for create artifact thumbnail on android in internal storage";

    protected override string OnGetRootPath() => "/storage/emulated/0/";
}
