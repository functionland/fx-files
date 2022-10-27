namespace Functionland.FxFiles.Client.Shared.Pages.FileViewer;

public partial class VideoFileViewerPage : IFileViewerPage
{
    public string Path { get; set; } = "D:\\test.mp4";
    public string PlayIcon { get; set; } = "_content/Functionland.FxFiles.Client.Shared/images/icons/videoViewer/pause.svg";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("FxVideo.initVideo");
        }
    }

    private void HandleToolbarBack()
    {
        NavigationManager.NavigateTo("settings", false, true);
    }

    private async Task OnBackwardClick()
    {
        await JSRuntime.InvokeVoidAsync("FxVideo.backward");
    }

    private async Task OnTogglePlayClick()
    {
        if (PlayIcon.Contains("play"))
        {
            PlayIcon = "_content/Functionland.FxFiles.Client.Shared/images/icons/videoViewer/pause.svg";
        }
        else
        {
            PlayIcon = "_content/Functionland.FxFiles.Client.Shared/images/icons/videoViewer/play.svg";
        }

        await JSRuntime.InvokeVoidAsync("FxVideo.togglePlay");
    }

    private async Task OnForwardClick()
    {
        await JSRuntime.InvokeVoidAsync("FxVideo.forward");
    }

    private async Task OnPictureInPictureClick()
    {
        await JSRuntime.InvokeVoidAsync("FxVideo.togglePictureInPicture");
    }
}