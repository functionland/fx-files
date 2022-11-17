namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

public abstract class PdfThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    private TFileService FileService { get; set; }
    protected PdfThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService) 
        : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    protected override async Task<FsArtifact?> OnGetArtifactAsync(string testRoot, string fileNameWithoutExtension, CancellationToken? cancellationToken = null)
    {
        var fullPath = Path.ChangeExtension(Path.Combine(testRoot, fileNameWithoutExtension), ".pdf");
        return await GetFsArtifactAsync(fullPath, "", cancellationToken);
    }

    protected override Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        return Task.CompletedTask;
    }

    private async Task<FsArtifact> GetFsArtifactAsync(string fullPath, string webUrl, CancellationToken? cancellationToken = null)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(webUrl);

        using var stream = await response.Content.ReadAsStreamAsync();

        var artifact = await FileService.CreateFileAsync(fullPath, stream, cancellationToken);
        return artifact;
    }

}
