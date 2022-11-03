namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }

    [AutoInject] private IZipService _zipService = default!;

    private FsArtifact _currentInnerZipArtifact =
        new FsArtifact("", "", FsArtifactType.Folder, FsFileProviderType.InternalMemory);
    private string? _password;


    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();
    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;


    protected override async Task OnInitAsync()
    {
        await LoadAllArtifactsAsync();
        await base.OnInitAsync();

        if (true)
        {
            // Get password from modal if required.
        }

        await LoadAllArtifactsAsync();
    }

    // Get the list of artifacts to display in the explorer
    private async Task LoadAllArtifactsAsync()
    {
        if (CurrentArtifact is null)
        {
            return;
        }

        var children = await _zipService.ViewZipFileAsync(CurrentArtifact.FullPath,
            _currentInnerZipArtifact?.FullPath ?? "", _password);
        _displayedArtifacts = children;
    }

    // Extract the artifact to the current directory
    private async Task ExtractArtifactsAsync()
    {

    }

    private async Task HandleBackAsync()
    {
        if (_currentInnerZipArtifact.FullPath == "")
        {
            // Navigate back.
        }
        else
        {
            _currentInnerZipArtifact = GetParent(_currentInnerZipArtifact);
            await LoadAllArtifactsAsync();
        }


    }

    private FsArtifact GetParent(FsArtifact artifact)
    {
        return artifact;
    }
}