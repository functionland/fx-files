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


    public Task<bool> IsSupportedAsync(string artrifactPath, IFileService fileService, CancellationToken? cancellationToken = null)
    {
        return OnIsSupportedAsync(artrifactPath, fileService, cancellationToken);
    }

    public async Task ViewAsync(string artrifactPath, IFileService fileService, string returnUrl)
    {
        var pageName = typeof(TViewrPage).Name;
        var encodedPath = WebUtility.UrlEncode(artrifactPath);

        var encodedReturnUrl = WebUtility.UrlEncode(returnUrl);
        var fileServiceName = fileService.GetType().Name;

        var route = $"FileViewers/{pageName}/{encodedPath}/{fileServiceName}/{encodedReturnUrl}";
        NavigationManager.NavigateTo(route);
    }

    protected abstract Task<bool> OnIsSupportedAsync(string artrifactPath, IFileService fileService, CancellationToken? cancellationToken = null);
}
