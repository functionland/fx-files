using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;
using Image = System.Drawing.Image;

namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations.Test;

public class WindowsPdfThumbnailPluginPlatformTest<TFileService> : PdfThumbnailPlatformTest<TFileService>
    where TFileService : IFileService
{
    IFileService FileService { get; set; }

    public WindowsPdfThumbnailPluginPlatformTest(IArtifactThumbnailService<TFileService> artifactThumbnailService, TFileService fileService)
        : base(artifactThumbnailService, fileService)
    {
        FileService = fileService;
    }

    public override string Title => $"WindowsPdfThumbnailPluginPlatformTest {typeof(TFileService).Name}";

    public override string Description => "Test for creating pdf thumbnail on windows";

    protected override string OnGetRootPath() => "C:\\";

    protected override (int width, int height) GetArtifactWidthAndHeight(string filePath)
    {
        var image = Image.FromFile(filePath);
        return (image.Width, image.Height);
    }
}
