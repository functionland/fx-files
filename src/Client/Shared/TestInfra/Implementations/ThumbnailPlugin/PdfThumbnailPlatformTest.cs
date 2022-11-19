using Functionland.FxFiles.Client.Shared.Enums;
using System.Diagnostics;

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


    protected override async Task OnPluginSpecificTestAsync(string testRoot, CancellationToken? cancellationToken = null)
    {
        var thumbnailScales = Enum.GetValues<ThumbnailScale>();

        var pdfFullPath = Path.ChangeExtension(Path.Combine(testRoot, $"firstPdf"), ".pdf");
        var pdfUrl_81_kb = "https://www.clickdimensions.com/links/TestPDFfile.pdf";
        var pdfArtifact_81_kb = await GetFsArtifactAsync(pdfFullPath, pdfUrl_81_kb, cancellationToken);

        foreach (var thumbnailScale in thumbnailScales)
        {
            var sw = new Stopwatch();
            sw.Start();
            var pdfThumbnailPath = await ArtifactThumbnailService.GetOrCreateThumbnailAsync(pdfArtifact_81_kb, thumbnailScale, cancellationToken);
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            Assert.IsNotNull(pdfArtifact_81_kb, $"Pdf Thumbnail created in {duration} ms. Size: {pdfArtifact_81_kb.SizeStr}");

            var (thumbnailWidth, thumbnailHeight) = GetArtifactWidthAndHeight(pdfThumbnailPath!);
            Assert.Success($"Pdf Thumbnai ratio: {thumbnailWidth}x{thumbnailHeight}, ThumbnailScale: {thumbnailScale}.");
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
