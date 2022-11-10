using Functionland.FxFiles.Client.Shared.TestInfra.Implementations.ThumbnailPlugin;

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

    public override string Description => "Test for create pdf thumbnail on windows";

    protected override string OnGetRootPath() => "C:\\";

    protected override (int width, int height) GetArtifactWidthAndHeight(string pdfPath)
    {
        //ToDo: Grab only the first page of pdf file from its path.
        throw new NotImplementedException();
    }
}
