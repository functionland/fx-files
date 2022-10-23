using Functionland.FxFiles.Client.Shared.Pages.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;

public class TextFileViewer : BlazorFileViewer<TextFileViewerPage>
{
    public TextFileViewer(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    protected override async Task<bool> OnIsSupportedAsync(string artrifactPath,
                                                           IFileService fileService,
                                                           CancellationToken? cancellationToken = null)
        => new string[] { ".txt" }.Contains(Path.GetExtension(artrifactPath));
}
