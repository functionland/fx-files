using Functionland.FxFiles.Client.Shared.Models;

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
    private ArtifactSelectionModal? _artifactSelectionModalRef;

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
            var destionationPath = await GetDestionationPathAsync();
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?>(CurrentArtifact, _selectedArtifacts, destionationPath);
            await OnExtract.InvokeAsync(extractTuple);
        }

        await HandleBackAsync(true);
    }

    private async Task HandleExtractArtifactAsync(FsArtifact artifact)
    {
        if (CurrentArtifact != null)
        {
            string? destinationPath = await GetDestionationPathAsync();
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
            string? destinationPath = await GetDestionationPathAsync();
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?>(CurrentArtifact, null, destinationPath);
            await OnExtract.InvokeAsync(extractTuple);
        }

        await HandleBackAsync(true);
    }

    private async Task<string?> GetDestionationPathAsync()
    {
        if (_artifactSelectionModalRef is null)
            return null;

        ArtifactActionResult actionResult = new()
        {
            ActionType = ArtifactActionType.Extract,
            Artifacts = null
        };

        var routeArtifact = await FileService.GetArtifactAsync(CurrentArtifact?.ParentFullPath);
        var result = await _artifactSelectionModalRef.ShowAsync(routeArtifact, actionResult);

        if (result.ResultType == ArtifactSelectionResultType.Cancel)
            return null;

        var destionationPath = result.SelectedArtifacts.FirstOrDefault()?.FullPath;
        return destionationPath;
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