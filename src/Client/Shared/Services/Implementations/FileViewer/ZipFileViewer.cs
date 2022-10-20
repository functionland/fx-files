using Functionland.FxFiles.Client.Shared.Pages.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;

public class ZipFileViewer : BlazorFileViewer<ZipFileViewerPage>
{
    public ZipFileViewer(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    protected override bool OnIsExtenstionSupported(string artrifactPath, IFileService fileService) 
        => new string[] { ".zip", ".rar" }.Contains(Path.GetExtension(artrifactPath));
}
