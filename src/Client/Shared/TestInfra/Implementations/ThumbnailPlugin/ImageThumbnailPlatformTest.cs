using System.Diagnostics;

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

    //This piece of code works fine but is ugly as hell, and needs some serious refactoring.
    protected override async Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        var thumbnailScales = Enum.GetValues<ThumbnailScale>();

        //9_kb_image
        var fullPath_9_kb = Path.ChangeExtension(Path.Combine(testRoot, $"image_9_kb"), ".jpg");
        var image_9_kb = "https://www.digikala.com/mag/wp-content/uploads/2020/08/DK-rebranding2.jpg";
        var imageArtifact_9_kb = await GetFsArtifactAsync(fullPath_9_kb, image_9_kb, cancellationToken);

        var (firstlWidth, firstHeight) = GetArtifactWidthAndHeight(fullPath_9_kb);
        foreach (var thumbnailScale in thumbnailScales)
        {
            var sw = new Stopwatch();
            sw.Start();
            var image_9_kb_thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(imageArtifact_9_kb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(image_9_kb_thumbnailPath, $"Image Thumbnail created in {duration} ms. Size: {imageArtifact_9_kb.SizeStr}");

            var (thumbnailWidth, thumbnailHeight) = GetArtifactWidthAndHeight(image_9_kb_thumbnailPath!);
            Assert.Success($"Artifact ratio: {firstlWidth}x{firstHeight}, Thumbnail ratio: {thumbnailWidth}x{thumbnailHeight}, ThumbnailScale: {thumbnailScale}.");
        }

        //293_kb_image
        var fullPath_293_kb = Path.ChangeExtension(Path.Combine(testRoot, $"image_293_kb"), ".jpg");
        var image_293_kb = "https://wallpaperaccess.com/full/1356284.jpg";
        var imageArtifact_293_kb = await GetFsArtifactAsync(fullPath_293_kb, image_293_kb, cancellationToken);

        var (secondWidth, secondHeight) = GetArtifactWidthAndHeight(fullPath_293_kb);
        foreach (var thumbnailScale in thumbnailScales)
        {
            var sw = new Stopwatch();
            sw.Start();
            var image_293_kb_thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(imageArtifact_293_kb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(image_293_kb_thumbnailPath, $"Image Thumbnail created in {duration} ms. Size: {imageArtifact_293_kb.SizeStr}");

            var (thumbnailWidth, thumbnailHeight) = GetArtifactWidthAndHeight(image_293_kb_thumbnailPath!);
            Assert.Success($"Artifact ratio: {secondWidth}x{secondHeight}, Thumbnail ratio: {thumbnailWidth}x{thumbnailHeight}, ThumbnailScale: {thumbnailScale}.");
        }
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
