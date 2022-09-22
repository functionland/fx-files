using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;
    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _artifacts = new();

    [Parameter] public IPinService PinService { get; set; } = default!;

    [Parameter] public IFileService FileService { get; set; } = default!;

    protected override async Task OnInitAsync()
    {
        await LoadPinsAsync();

        await LoadChildrenArtifactsAsync();

        await base.OnInitAsync();
    }

    private async Task LoadPinsAsync()
    {
        var allPins = PinService.GetPinnedArtifactsAsync(null);

        await foreach (var item in allPins)
        {
            _pins.Add(item);
        }
    }

    private async Task LoadChildrenArtifactsAsync(FsArtifact? parentArtifact = null)
    {
        var allFiles = FileService.GetArtifactsAsync(parentArtifact?.FullPath);

        await foreach (var item in allFiles)
        {
            _artifacts.Add(item);
        }
    }

    private async Task HandleSelectArtifact(FsArtifact artifact)
    {
        _currentArtifact = artifact;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        // load current artifacts
    }

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }
}
