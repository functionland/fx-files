using Functionland.FxFiles.Client.Shared.Pages.FileViewer;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;

public class TextFileViewer : BlazorFileViewer<TextFileViewerPage>
{
    public TextFileViewer(NavigationManager navigationManager) : base(navigationManager)
    {
    }

    protected override bool OnIsExtenstionSupported(FsArtifact fsArtifact) 
        => new string[] { ".txt" }.Contains(Path.GetExtension(fsArtifact.FullPath));
}
