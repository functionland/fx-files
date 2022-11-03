namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }

    [AutoInject] private IZipService _zipService = default!;

    private FsArtifact _currentInnerZipArtifact =
        new FsArtifact("", "", FsArtifactType.Folder, FsFileProviderType.InternalMemory);

    private string? _password;
    private CancellationTokenSource _cancellationTokenSource;


    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();
    private List<FsArtifact> _allZipFileEntity = new();
    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;


    protected override async Task OnInitAsync()
    {
        if (true)
        {
            // Get password from modal if required.
        }

        await LoadAllArtifactsAsync();
        await base.OnInitAsync();
    }

    // Get the list of artifacts to display in the explorer
    private async Task LoadAllArtifactsAsync()
    {
        if (CurrentArtifact is null)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        var children = await _zipService.GetAllInnerZippedArtifactsAsync(CurrentArtifact.FullPath, _password, token);

        _displayedArtifacts = children;
    }

    private async Task LoadInnerZipArtifactsAsync(FsArtifact artifact)
    {
        if (string.IsNullOrEmpty(artifact.ParentFullPath))
        {
            _displayedArtifacts = _allZipFileEntity;
        }
        else
        {
            _displayedArtifacts = _allZipFileEntity.Where(x => x.ParentFullPath == artifact.FullPath).ToList();
        }
    }

    // Extract the artifact to the current directory
    private async Task HandleExtractArtifactsAsync(List<FsArtifact> artifacts)
    {
        // full path, destination path, destination name, override if exists , password
        //await _zipService.ExtractZipFileAsync(CurrentArtifact.FullPath, artifacts, _password);
    }

    private async Task HandleExtractArtifactsAsync(FsArtifact artifacts)
    {
        // full path, destination path, destination name, override if exists , password
        //await _zipService.ExtractZipFileAsync(CurrentArtifact.FullPath, artifacts, _password);
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

    private async Task HandleArtifactClickAsync(FsArtifact artifact)
    {
        if (artifact.ArtifactType == FsArtifactType.Folder)
        {
            _currentInnerZipArtifact = artifact;
            await LoadAllArtifactsAsync();
        }
    }

    private void HandleSelectAllArtifact()
    {
        _displayedArtifacts.ForEach(x => x.IsSelected = true);
        _selectedArtifacts = _displayedArtifacts;
        ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
    }
}