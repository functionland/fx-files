using Functionland.FxFiles.Client.Shared.Pages.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;

public class ZipFileViewer : BlazorFileViewer<ZipFileViewerPage>
{
    public ZipFileViewer(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    protected override async Task<bool> OnIsSupportedAsync(string artrifactPath, IFileService fileService, CancellationToken? cancellationToken = null)
        => new string[] { ".zip", ".rar" }.Contains(Path.GetExtension(artrifactPath));
}
