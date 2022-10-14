namespace Functionland.FxFiles.Client.Shared.Services.Implementations.ShareService;

public partial class FakeShareServiceFactory
{
    [AutoInject] public IFulaFileService FulaFileService { get; set; } = default!;
    public FakeShareService CreateIsSharedWithMeArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var isSharedByMeArtifactPermissionInfo = new List<ArtifactPermissionInfo>();

        return new FakeShareService(
            FulaFileService,
            isSharedByMeArtifactPermissionInfo,
            new List<ArtifactPermissionInfo>
            {
               ArtifactPermissionInfo("/shared/Document/docs.txt", "DId", ArtifactPermissionLevel.Read),
               ArtifactPermissionInfo("/shared/Document/docs.pdf", "DId", ArtifactPermissionLevel.Write)
            },
            actionLatency,
            enumerationLatency);
    }

    public FakeShareService CreateIsSharedByMeArtifacts(TimeSpan? actionLatency = null, TimeSpan? enumerationLatency = null)
    {
        var isSharedByMeArtifactPermissionInfo = new List<ArtifactPermissionInfo>();

        return new FakeShareService(
            FulaFileService,
            isSharedByMeArtifactPermissionInfo,
            new List<ArtifactPermissionInfo>
            {
               ArtifactPermissionInfo("/Files/video", "DId 123", ArtifactPermissionLevel.Read),
               ArtifactPermissionInfo("/Files/Audio", "DId 1234", ArtifactPermissionLevel.Read)
            },
            actionLatency,
            enumerationLatency);
    }

    private static ArtifactPermissionInfo ArtifactPermissionInfo(string fullPath, string dId, ArtifactPermissionLevel PermissionLevel)
    {
        var artifactPermissionInfo = new ArtifactPermissionInfo()
        {
            FullPath = fullPath,
            DId = dId,
            PermissionLevel = PermissionLevel
        };

        return artifactPermissionInfo;
    }

}
