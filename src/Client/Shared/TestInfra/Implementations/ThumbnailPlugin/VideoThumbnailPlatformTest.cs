using System.Diagnostics;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

public abstract class VideoThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    private TFileService FileService { get; set; }
    protected VideoThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
        : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    protected override async Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        var thumbnailScales = Enum.GetValues<ThumbnailScale>();

        //374_kb_Mp4_video
        var fullPath_374_kb = Path.ChangeExtension(Path.Combine(testRoot, $"video_531_kb"), ".mp4");
        var videoUrl_374_kb = "http://techslides.com/demos/sample-videos/small.mp4";

        var client = new HttpClient();
        var response = await client.GetAsync(videoUrl_374_kb);

        using var stream = await response.Content.ReadAsStreamAsync();
        var videoArtifact_374_kb = await FileService.CreateFileAsync(fullPath_374_kb, stream, cancellationToken);

        foreach (var thumbnailScale in thumbnailScales)
        {
            var sw = new Stopwatch();
            sw.Start();
            var video_374_kb_thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(videoArtifact_374_kb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(video_374_kb_thumbnailPath, $"Video Thumbnail created in {duration} ms. Size: {videoArtifact_374_kb.SizeStr}");

            var (thumbnailWidth, thumbnailHeight) = GetArtifactWidthAndHeight(video_374_kb_thumbnailPath!);
            Assert.Success($"Video Thumbnail ratio: {thumbnailWidth}x{thumbnailHeight}, ThumbnailScale: {thumbnailScale}.");
        }
    }
}
