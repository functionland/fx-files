namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [AutoInject] private IZipService _zipService = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<Tuple<FsArtifact, List<FsArtifact>?, string?, string?>> OnExtract { get; set; }
    [Parameter] public FileViewerResultType FileViewerResult { get; set; }

    // Modals
    private InputModal? _passwordModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;

    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;

    private readonly CancellationTokenSource _cancellationTokenSource = new();


    private FsArtifact _currentInnerZipArtifact =
        new(string.Empty, string.Empty, FsArtifactType.Folder, FsFileProviderType.InternalMemory);

    private string? _password = null;

    private List<FsArtifact> _displayedArtifacts = new();

    private List<FsArtifact> _selectedArtifacts = new();

    private List<FsArtifact> _allZipFileEntities = new();


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
        catch (InvalidPasswordException)
        {
            await GetArtifactPassword();
        }
        catch (Exception exception)
        {
            await HandleBackAsync(true);
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task GetArtifactPassword()
    {
        if (_passwordModalRef is null)
            return;

        var token = _cancellationTokenSource.Token;
        var extractBtnTitle = Localizer.GetString(AppStrings.Extract);
        var extractPasswordModalTitle = Localizer.GetString(AppStrings.ExtractPasswordModalTitle);
        var extractPasswordModalLabel = Localizer.GetString(AppStrings.Password);
        var passwordResult = await _passwordModalRef.ShowAsync(extractPasswordModalTitle, string.Empty, string.Empty,
            string.Empty, extractBtnTitle, extractPasswordModalLabel);

        if (passwordResult.ResultType == InputModalResultType.Cancel)
        {
            await HandleBackAsync(true);
            return;
        }

        _password = passwordResult.Result;
        try
        {
            if (CurrentArtifact != null)
            {
                await _zipService.GetAllArtifactsAsync(CurrentArtifact.FullPath, _password, token);
            }
        }
        catch (NotSupportedEncryptedFileException exception)
        {
            await HandleBackAsync(true);
            ExceptionHandler.Handle(exception);
        }
        catch (Exception exception)
        {
            await HandleBackAsync(true);
            ExceptionHandler.Handle(exception);
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
        _displayedArtifacts = _allZipFileEntities.Where(a => a.ParentFullPath == artifact.FullPath).OrderByDescending(a => a.ArtifactType == FsArtifactType.Folder).ToList();
    }

    private async Task HandleExtractArtifactsAsync(List<FsArtifact> artifacts)
    {
        var destinationPath = await GetDestinationPathAsync(artifacts);
        if (CurrentArtifact != null && destinationPath != null)
        {
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?, string?>(CurrentArtifact, _selectedArtifacts, destinationPath, _password);
            await OnExtract.InvokeAsync(extractTuple);
            if (FileViewerResult == FileViewerResultType.Success)
            {
                await HandleBackAsync(true);
            }
        }
    }

    private async Task HandleExtractArtifactAsync(FsArtifact artifact)
    {
        var destinationPath = await GetDestinationPathAsync(new List<FsArtifact> { artifact });
        if (CurrentArtifact != null && destinationPath != null)
        {
            var singleArtifactList = new List<FsArtifact> { artifact };
            var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?, string?>(CurrentArtifact, singleArtifactList, destinationPath, _password);
            await OnExtract.InvokeAsync(extractTuple);
            if (FileViewerResult == FileViewerResultType.Success)
            {
                await HandleBackAsync(true);
            }
        }
    }

    private async Task HandleExtractCurrentArtifactAsync()
    {
        if (CurrentArtifact != null)
        {
            var destinationPath = await GetDestinationPathAsync(new List<FsArtifact> { CurrentArtifact });
            if (CurrentArtifact != null && destinationPath != null)
            {
                var extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?, string?>(CurrentArtifact, null, destinationPath, _password);
                await OnExtract.InvokeAsync(extractTuple);
                if (FileViewerResult == FileViewerResultType.Success)
                {
                    await HandleBackAsync(true);
                }
            }
        }
    }

    private async Task<string?> GetDestinationPathAsync(List<FsArtifact> artifacts)
    {
        if (_artifactSelectionModalRef is null)
            return null;

        ArtifactActionResult actionResult = new()
        {
            ActionType = ArtifactActionType.Extract,
            Artifacts = artifacts
        };

        var routeArtifact = await FileService.GetArtifactAsync(CurrentArtifact?.ParentFullPath);
        var result = await _artifactSelectionModalRef.ShowAsync(routeArtifact, actionResult);

        if (result.ResultType == ArtifactSelectionResultType.Cancel)
            return null;

        var destinationPath = result.SelectedArtifacts.FirstOrDefault()?.FullPath;
        return destinationPath;
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
        if (artifact.ArtifactType != FsArtifactType.Folder)
            return;

        _currentInnerZipArtifact = artifact;
        DisplayChildrenArtifacts(_currentInnerZipArtifact);
    }

    private void HandleSelectAllArtifact()
    {
        _displayedArtifacts.ForEach(x => x.IsSelected = true);
        _selectedArtifacts = _displayedArtifacts.ToList();
        ChangeArtifactExplorerMode(ArtifactExplorerMode.SelectArtifact);
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