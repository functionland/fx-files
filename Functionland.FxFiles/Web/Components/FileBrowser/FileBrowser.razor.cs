using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;
    private IEnumerable<FsArtifact> _pins = default!;
    private IEnumerable<FsArtifact> _artifacts = default!;

    [Parameter] public IPinService PinService { get; set; } = default!;

    [Parameter] public IFileService FileService { get; set; } = default!;

    //public List<FileCardConfig> PinnedCards = new List<FileCardConfig>
    //    {
    //        new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size"),
    //        new FileCardConfig(true, true, true, "Fx Land", ".mp3", "date", "file size"),
    //        new FileCardConfig(true, true, true, "doument", ".pdf", "date", "file size"),
    //        new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size")
    //    };

    protected override async Task OnInitAsync()
    {
        await LoadPinsAsync();

        await LoadChildrenArtifactsAsync();

        await base.OnInitAsync();
    }

    private async Task LoadPinsAsync()
    {
        var allPins = PinService.GetPinnedArtifactsAsync(null);

        var pins = new List<FsArtifact>();

        await foreach (var item in allPins)
        {
            pins.Add(item);
        }

        _pins = pins;
    }

    private async Task LoadChildrenArtifactsAsync(FsArtifact? parentArtifact = null)
    {
        var allFiles = FileService.GetArtifactsAsync(parentArtifact?.FullPath);

        var artifacts = new List<FsArtifact>();

        await foreach (var item in allFiles)
        {
            artifacts.Add(item);
        }

        _artifacts = artifacts;
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
