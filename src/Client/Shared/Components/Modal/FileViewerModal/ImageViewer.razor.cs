namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ImageViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }

    protected override Task OnInitAsync()
    {
        GoBackService.SetState(HandleBackAsync, true, false);
        return base.OnInitAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("ImagePinchZoom");
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.Storage,
                IFulaFileService => PathProtocol.Fula,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };

    private async Task HandlePinAsync()
    {
        if (CurrentArtifact is null) return;

        CurrentArtifact.IsPinned = true;
        var pinArtifact = new List<FsArtifact> { CurrentArtifact };
        await OnPin.InvokeAsync(pinArtifact);
    }

    private async Task HandleUnpinAsync()
    {
        if (CurrentArtifact is null) return;

        CurrentArtifact.IsPinned = false;
        var unPinArtifact = new List<FsArtifact> { CurrentArtifact };
        await OnUnpin.InvokeAsync(unPinArtifact);
    }

    private async Task HandleOptionClickAsync()
    {
        await OnOptionClick.InvokeAsync(CurrentArtifact);
    }

    private async Task HandleBackAsync()
    {
        CurrentArtifact = null;
        await OnBack.InvokeAsync();
    }
}