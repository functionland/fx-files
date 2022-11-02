using System.Reflection.Metadata;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class TextViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }

    private string Text { get; set; } = string.Empty;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _ = GetTextAsync();
        }

        base.OnAfterRender(firstRender);
    }

    private async Task HandlePinAsync()
    {
        if (CurrentArtifact is null) return;

        await OnPin.InvokeAsync(new List<FsArtifact>() { CurrentArtifact });
    }

    private async Task HandleUnpinAsync()
    {
        if (CurrentArtifact is null) return;

        await OnUnpin.InvokeAsync(new List<FsArtifact>() { CurrentArtifact });
    }

    private async Task HandleOptionClickAsync()
    {
        if (CurrentArtifact is null) return;

        await OnOptionClick.InvokeAsync(CurrentArtifact);
    }

    private async Task GetTextAsync()
    {
        if (CurrentArtifact?.FullPath == null) return;

        using var stream = await FileService.GetFileContentAsync(CurrentArtifact.FullPath);
        using var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
        Text = streamReader.ReadToEnd();
        await InvokeAsync(() => StateHasChanged());
    }
}