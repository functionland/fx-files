using System.Reflection.Metadata;
using System.Text;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class TextViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnPin { get; set; }
    [Parameter] public EventCallback<List<FsArtifact>> OnUnpin { get; set; }
    [Parameter] public EventCallback<FsArtifact> OnOptionClick { get; set; }

    private StringBuilder Text { get; set; } = new();

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
        using var streamReader = new StreamReader(stream);
        while (streamReader.ReadLine() is string line)
        {
            Text.AppendLine(line);
            await InvokeAsync(() => StateHasChanged());
        }
    }
}