namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

public abstract class ImageThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    private TFileService FileService { get; set; }
    protected ImageThumbnailPlatformTest(
        IArtifactThumbnailService<TFileService> artifactThumbnailService,
        TFileService fileService) : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    protected override async Task<FsArtifact> CreateArtifactAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        using FileStream fs = File.Open(Path.Combine(GetSampleFileLocalPath(), "fake-pic.jpg"), FileMode.Open);

        var createdImageArtifact = await FileService.CreateFileAsync($@"{testRoot}\1.jpg", fs);

        return createdImageArtifact;
    }
}
