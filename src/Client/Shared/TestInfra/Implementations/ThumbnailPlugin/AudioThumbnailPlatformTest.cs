using System.Diagnostics;

namespace Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

public abstract class AudioThumbnailPlatformTest<TFileService> : ArtifactThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    private TFileService FileService { get; set; }
    protected AudioThumbnailPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
        : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    protected override async Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        var thumbnailScales = Enum.GetValues<ThumbnailScale>();

        //8_mb_audio
        var fullPath_8_mb = Path.ChangeExtension(Path.Combine(testRoot, $"audio_8_mb"), ".mp4");
        var audioUrl_8_mb = "https://www.learningcontainer.com/download/sample-mp3-file/";

        var client = new HttpClient();
        var response = await client.GetAsync(audioUrl_8_mb);

        using var stream = await response.Content.ReadAsStreamAsync();
        var audioArtifact_8_mb = await FileService.CreateFileAsync(fullPath_8_mb, stream, cancellationToken);

        foreach (var thumbnailScale in thumbnailScales)
        {
            var sw = new Stopwatch();
            sw.Start();
            var audio_8_mb_thumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(audioArtifact_8_mb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(audio_8_mb_thumbnailPath, $"Audio Thumbnail created in {duration} ms. Size: {audioArtifact_8_mb.SizeStr}");

            var (thumbnailWidth, thumbnailHeight) = GetArtifactWidthAndHeight(audio_8_mb_thumbnailPath!);
            Assert.Success($"Audio Thumbnail ratio: {thumbnailWidth}x{thumbnailHeight}, ThumbnailScale: {thumbnailScale}.");
        }
    }
}
