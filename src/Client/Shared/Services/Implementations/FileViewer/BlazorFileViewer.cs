using System.Net;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer;

public abstract class BlazorFileViewer<TViewrPage> : IFileViewer
    where TViewrPage : IFileViewerPage
{
    protected NavigationManager NavigationManager = default!;
    public BlazorFileViewer(NavigationManager navigationManager)
    {
        NavigationManager = navigationManager;
    }
    public bool IsExtenstionSupported(FsArtifact fsArtifact)
    {
        return OnIsExtenstionSupported(fsArtifact);
    }

    public async Task ViewAsync(FsArtifact artrifact, IFileService fileService, string returnUrl)
    {
        var pageName = typeof(TViewrPage).Name;
        var encodedPath = WebUtility.UrlEncode(artrifact.FullPath);

        var encodedReturnUrl = WebUtility.UrlEncode(returnUrl);
        var fileServiceName = fileService.GetType().Name;

        var rout = $"FileViewers/{pageName}/{encodedPath}/{fileServiceName}/{encodedReturnUrl}";
        NavigationManager.NavigateTo(rout);
    }

    protected abstract bool OnIsExtenstionSupported(FsArtifact fsArtifact);
}
