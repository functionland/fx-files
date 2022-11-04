namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<Tuple<FsArtifact, List<FsArtifact>?, string?>> OnExtract { get; set; }

    [AutoInject] private IZipService _zipService = default!;

    private FsArtifact _currentInnerZipArtifact =
        new(string.Empty, string.Empty, FsArtifactType.Folder, FsFileProviderType.InternalMemory);

    private string? _password = null;
    private CancellationTokenSource _cancellationTokenSource = new();


    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();
    private List<FsArtifact> _allZipFileEntities = new();
    private InputModal? _passwordModalRef;
    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitialZipViewerAsync();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task InitialZipViewerAsync()
    {
        if (CurrentArtifact is null)
            return;

        try
        {
            await LoadAllArtifactsAsync();

            DisplayChildrenArtifacts(_currentInnerZipArtifact);
        }
        catch (Exception e)
        {
            await HandleBackAsync(true);
            ExceptionHandler.Handle(e);
        }
    }

    private async Task LoadAllArtifactsAsync()
    {
        if (CurrentArtifact is null)
            throw new InvalidOperationException("Current artifact can not be null.");

        var token = _cancellationTokenSource.Token;

        _allZipFileEntities = await _zipService.GetAllArtifactsAsync(CurrentArtifact.FullPath, _password, token);
    }

    private void DisplayChildrenArtifacts(FsArtifact artifact)
    {
        _displayedArtifacts = _allZipFileEntities.Where(a => a.ParentFullPath == artifact.FullPath).ToList();
    }

    private async Task HandleExtractArtifactsAsync(List<FsArtifact> artifacts)
    {
        if (CurrentArtifact != null)
        {
            //TODO : Handle extract artifacts to a specific folder
            string? destinationPath = null;
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?>(CurrentArtifact, _selectedArtifacts, destinationPath);
            await OnExtract.InvokeAsync(extractTuple);
        }

        await HandleBackAsync(true);
    }

    private async Task HandleExtractArtifactAsync(FsArtifact artifact)
    {
        if (CurrentArtifact != null)
        {
            //TODO : Handle extract artifacts to a specific folder
            string? destinationPath = null;
            var singleArtifactList = new List<FsArtifact> { artifact };
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?>(CurrentArtifact, singleArtifactList, destinationPath);
            await OnExtract.InvokeAsync(extractTuple);
        }

        await HandleBackAsync(true);
    }

    private async Task HandleExtractCurrentArtifactAsync()
    {
        if (CurrentArtifact != null)
        {
            //TODO : Handle extract artifacts to a specific folder
            string? destinationPath = null;
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?>(CurrentArtifact, null, destinationPath);
            await OnExtract.InvokeAsync(extractTuple);
        }

        await HandleBackAsync(true);
    }

    private async Task HandleBackAsync(bool shouldExit = false)
    {
        if (_currentInnerZipArtifact.FullPath == string.Empty || shouldExit)
        {
            _cancellationTokenSource.Cancel();
            await OnBack.InvokeAsync();
        }
        else
        {
            _currentInnerZipArtifact = GetParent(_currentInnerZipArtifact);
            DisplayChildrenArtifacts(_currentInnerZipArtifact);
        }
    }

    private FsArtifact GetParent(FsArtifact artifact)
    {
        var parentArtifact = _allZipFileEntities.Find(a => a.FullPath == artifact.ParentFullPath);
        return parentArtifact ?? new FsArtifact("", "", FsArtifactType.Folder, FsFileProviderType.InternalMemory);
    }

    private void HandleArtifactClick(FsArtifact artifact)
    {
        if (artifact.ArtifactType == FsArtifactType.Folder)
        {
            _currentInnerZipArtifact = artifact;
            DisplayChildrenArtifacts(_currentInnerZipArtifact);
        }
    }

    private void HandleSelectAllArtifact()
    {
        _displayedArtifacts.ForEach(x => x.IsSelected = true);
        _selectedArtifacts = _displayedArtifacts.ToList();
        ChangeArtifactExplorerMode(ArtifactExplorerMode.SelectArtifact);
    }

    private void HandleSelectArtifact(FsArtifact artifact)
    {
        var selectedArtifact = _displayedArtifacts.FirstOrDefault(a => a.FullPath == artifact.FullPath);
        if (_selectedArtifacts.Any(a => a.FullPath == artifact.FullPath))
        {
            _selectedArtifacts.Remove(artifact);

            if (selectedArtifact is not null)
            {
                selectedArtifact.IsSelected = false;
            }
        }
        else
        {
            _selectedArtifacts.Add(artifact);

            if (selectedArtifact is not null)
            {
                selectedArtifact.IsSelected = true;
            }
        }

        ChangeArtifactExplorerMode(_selectedArtifacts.Count > 0
            ? ArtifactExplorerMode.SelectArtifact
            : ArtifactExplorerMode.Normal);
    }

    private void ChangeArtifactExplorerMode(ArtifactExplorerMode explorerMode)
    {
        ArtifactExplorerMode = explorerMode;
    }

    private void CancelSelectionMode()
    {
        _displayedArtifacts.ForEach(x => x.IsSelected = false);
        _selectedArtifacts.Clear();
        DisplayChildrenArtifacts(_currentInnerZipArtifact);
        ChangeArtifactExplorerMode(ArtifactExplorerMode.Normal);
    }
}