namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [AutoInject] private IZipService _zipService = default!;
    [AutoInject] protected IStringLocalizer<AppStrings> Localizer = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public FileViewerResultType FileViewerResult { get; set; }

    // Modals
    private InputModal? _folderNameInputModalRef;
    private InputModal? _passwordModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private Extractor? _extractorModalRef;

    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;

    private readonly CancellationTokenSource _cancellationTokenSource = new();


    private FsArtifact _currentInnerZipArtifact =
        new(string.Empty, string.Empty, FsArtifactType.Folder, FsFileProviderType.InternalMemory);

    private string? _password = null;

    private List<FsArtifact> _displayedArtifacts = new();

    private List<FsArtifact> _selectedArtifacts = new();

    private List<FsArtifact> _allZipFileEntities = new();

    protected override Task OnInitAsync()
    {
        GoBackService.OnInit((async Task () =>
        {
            await HandleBackAsync();
            await Task.CompletedTask;
        }), true, false);
        return base.OnInitAsync();
    }

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
        catch (NotSupportedEncryptedFileException)
        {
            FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorTitle)), Localizer.GetString(nameof(AppStrings.NotSupportedEncryptedFileException)), FxToastType.Error);
            await HandleBackAsync(true);
        }
        catch (Exception)
        {
            await HandleBackAsync(true);
            throw;
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
        _displayedArtifacts = (
             from innerArtifact in _allZipFileEntities
             where innerArtifact.ParentFullPath == artifact.FullPath
             orderby artifact.ArtifactType descending, artifact.Name ascending
             select innerArtifact
        ).ToList();
    }

    private void GetExtractResult(FileViewerResultType resultType)
    {
        FileViewerResult = resultType;
    }

    private async Task HandleExtractArtifactsAsync(List<FsArtifact> artifacts)
    {
        var destinationPath = await GetDestinationPathAsync(artifacts);
        if (CurrentArtifact != null && destinationPath != null)
        {
            if (_folderNameInputModalRef is null)
            {
                FileViewerResult = FileViewerResultType.Cancel;
                return;
            }

            var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
            var createFolder = Localizer.GetString(AppStrings.FolderName);
            var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
            var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

            var folderNameResult = await _folderNameInputModalRef?.ShowAsync(createFolder, string.Empty, folderName,
                newFolderPlaceholder, extractBtnTitle);


            if (folderNameResult.Result != null)
            {
                if (_extractorModalRef != null)
                {
                    await _extractorModalRef.ExtractZipAsync(CurrentArtifact.FullPath, destinationPath,
                        folderNameResult.Result,
                        password: _password, artifacts);
                }
            }

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
            if (_folderNameInputModalRef is null)
            {
                FileViewerResult = FileViewerResultType.Cancel;
                return;
            }

            var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
            var createFolder = Localizer.GetString(AppStrings.FolderName);
            var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
            var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

            var folderNameResult = await _folderNameInputModalRef?.ShowAsync(createFolder, string.Empty, folderName,
                newFolderPlaceholder, extractBtnTitle);


            if (folderNameResult.Result != null)
            {
                if (_extractorModalRef != null)
                {
                    await _extractorModalRef.ExtractZipAsync(CurrentArtifact.FullPath, destinationPath,
                        folderNameResult.Result,
                        password: _password, new List<FsArtifact> { artifact });
                }
            }

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
                if (_folderNameInputModalRef is null)
                {
                    FileViewerResult = FileViewerResultType.Cancel;
                    return;
                }

                var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
                var createFolder = Localizer.GetString(AppStrings.FolderName);
                var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
                var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

                var folderNameResult = await _folderNameInputModalRef?.ShowAsync(createFolder, string.Empty, folderName,
                    newFolderPlaceholder, extractBtnTitle);


                if (folderNameResult.Result != null)
                {
                    if (_extractorModalRef != null)
                    {
                        await _extractorModalRef.ExtractZipAsync(CurrentArtifact.FullPath, destinationPath,
                            folderNameResult.Result,
                            password: _password, null);
                    }
                }

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
        if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
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
        else if (ArtifactExplorerMode == ArtifactExplorerMode.Normal)
        {
            CancelSelectionMode();
        }
        StateHasChanged();
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