using Functionland.FxFiles.Client.Shared.Utils;
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

    protected override async Task<FsArtifact?> OnGetArtifactAsync(string testRoot, string fileNameWithoutExtension, CancellationToken? cancellationToken = null)
    {
        var fullPath = Path.ChangeExtension(Path.Combine(testRoot, fileNameWithoutExtension), ".jpg");
        return await GetFsArtifactAsync(fullPath, "https://www.digikala.com/mag/wp-content/uploads/2020/08/DK-rebranding2.jpg", cancellationToken);
    }

    protected override async Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        var thumbnailScales = Enum.GetValues<ThumbnailScale>();
        foreach (var thumbnailScale in thumbnailScales)
        {
            var image_293_kb = "https://wallpaperaccess.com/full/1356284.jpg";

            var fullPath_293_kb = Path.ChangeExtension(Path.Combine(testRoot, $"image_293_kb_{thumbnailScale}"), ".jpg");
            var fsArtifact_293_kb = await GetFsArtifactAsync(fullPath_293_kb, image_293_kb, cancellationToken);

            var (initialWidth, initilaHeight) = GetArtifactWidthAndHeight(fullPath_293_kb);


            var (expectedWidth, expectedHeight) = ImageUtils.ScaleImage(initialWidth, initilaHeight, thumbnailScale);

            var sw = new Stopwatch();
            sw.Start();
            var image_293_kb_thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(fsArtifact_293_kb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(image_293_kb_thumbnailPath, $"Image Thumbnail {thumbnailScale} created in {duration} ms");
            AssertThumbnailWidthAndHeight(expectedWidth, expectedHeight, image_293_kb_thumbnailPath!);
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
