namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ZipViewer : IFileViewerComponent
{
    [AutoInject] private IZipService _zipService = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public FsArtifact? CurrentArtifact { get; set; }
    [Parameter] public EventCallback OnBack { get; set; }
    [Parameter] public EventCallback<string> NavigationFolderCallback { get; set; }

    // Modals
    private InputModal? _folderNameInputModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private ExtractorBottomSheet? _extractorModalRef;

    private ArtifactExplorerMode ArtifactExplorerMode { get; set; } = ArtifactExplorerMode.Normal;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private ExtractorBottomSheetResult? ExtractorBottomSheetResult { get; set; }

    private FsArtifact _currentInnerZipArtifact =
        new(string.Empty, string.Empty, FsArtifactType.Folder, FsFileProviderType.InternalMemory);

    private List<FsArtifact> _displayedArtifacts = new();

    private List<FsArtifact> _selectedArtifacts = new();

    private List<FsArtifact> _allZipFileEntities = new();

    private bool _isZipViewerInLoading;

    protected override Task OnInitAsync()
    {
        SetGoBackDeviceButtonFunctionality();
        return base.OnInitAsync();
    }

    protected override async Task OnAfterFirstRenderAsync()
    {
        await base.OnAfterFirstRenderAsync();

        _isZipViewerInLoading = true;
        StateHasChanged();

        _ = Task.Run(async () =>
        {
            await InitialZipViewerAsync();
        });
    }

    private async Task InitialZipViewerAsync()
    {
        if (CurrentArtifact is null)
            return;

        await Task.Run(async () =>
        {
            await InvokeAsync(async () =>
            {
                try
                {
                    await LoadAllArtifactsAsync();
                    DisplayChildrenArtifacts(_currentInnerZipArtifact);
                }
                catch (NotSupportedEncryptedFileException)
                {
                    FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorTitle)),
                    Localizer.GetString(nameof(AppStrings.NotSupportedEncryptedFileException)), FxToastType.Error);
                    await HandleBackAsync(true);
                }
                catch (Exception)
                {
                    await HandleBackAsync(true);
                    throw;
                }
                finally
                {
                    _isZipViewerInLoading = false;
                    StateHasChanged();
                }
            });
        });       
    }

    private async Task LoadAllArtifactsAsync()
    {
        if (CurrentArtifact is null)
            throw new InvalidOperationException("Current artifact can not be null.");

        var token = _cancellationTokenSource.Token;

        _allZipFileEntities =
            await _zipService.GetAllArtifactsAsync(CurrentArtifact.FullPath, cancellationToken: token);
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

    private async Task HandleExtractArtifactsAsync(List<FsArtifact> artifacts)
    {
        var destinationPath = await GetDestinationPathAsync(artifacts);
        var destinationDirectory = destinationPath ?? CurrentArtifact?.FullPath;
        if (CurrentArtifact != null && destinationPath != null)
        {
            if (_folderNameInputModalRef is null)
            {
                return;
            }

            var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
            var createFolder = Localizer.GetString(AppStrings.FolderName);
            var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
            var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

            var folderNameResult = await _folderNameInputModalRef.ShowAsync(createFolder, string.Empty, folderName,
                newFolderPlaceholder, extractBtnTitle);

            if (folderNameResult.ResultType == InputModalResultType.Cancel)
            {
                SetGoBackDeviceButtonFunctionality();
                return;
            }

            var destinationFolderName = string.IsNullOrWhiteSpace(folderNameResult?.Result) == false
                ? folderNameResult.Result
                : folderName;

            if (folderNameResult?.Result != null && _extractorModalRef != null)
            {
                ExtractorBottomSheetResult = await _extractorModalRef.ShowAsync(CurrentArtifact.FullPath,
                    destinationPath,
                    folderNameResult.Result, artifacts);
            }

            switch (ExtractorBottomSheetResult?.ExtractorResult)
            {
                case ExtractorBottomSheetResultType.Cancel:
                    SetGoBackDeviceButtonFunctionality();
                    return;
                case ExtractorBottomSheetResultType.Success when destinationDirectory != null:
                    {
                        var destinationResultPath = Path.Combine(destinationDirectory, destinationFolderName);
                        await NavigationFolderCallback.InvokeAsync(destinationResultPath);
                        break;
                    }
                case null:
                    SetGoBackDeviceButtonFunctionality();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task HandleExtractArtifactAsync(FsArtifact artifact)
    {
        var destinationPath = await GetDestinationPathAsync(new List<FsArtifact> { artifact });
        var destinationDirectory = destinationPath ?? CurrentArtifact?.FullPath;
        if (CurrentArtifact != null && destinationPath != null)
        {
            if (_folderNameInputModalRef is null)
            {
                return;
            }

            var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
            var createFolder = Localizer.GetString(AppStrings.FolderName);
            var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
            var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

            var folderNameResult = await _folderNameInputModalRef.ShowAsync(createFolder, string.Empty, folderName,
                newFolderPlaceholder, extractBtnTitle);

            if (folderNameResult.ResultType == InputModalResultType.Cancel)
            {
                SetGoBackDeviceButtonFunctionality();
                return;
            }

            var destinationFolderName = folderNameResult.Result ?? folderName;


            if (_extractorModalRef != null)
            {
                ExtractorBottomSheetResult = await _extractorModalRef.ShowAsync(CurrentArtifact.FullPath,
                    destinationPath,
                    destinationFolderName, new List<FsArtifact> { artifact });
            }

            switch (ExtractorBottomSheetResult?.ExtractorResult)
            {
                case ExtractorBottomSheetResultType.Cancel:
                    SetGoBackDeviceButtonFunctionality();
                    return;
                case ExtractorBottomSheetResultType.Success when destinationDirectory != null:
                    {
                        var destinationResultPath = Path.Combine(destinationDirectory, destinationFolderName);
                        await NavigationFolderCallback.InvokeAsync(destinationResultPath);
                        break;
                    }
                case null:
                    SetGoBackDeviceButtonFunctionality();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task HandleExtractCurrentArtifactAsync()
    {
        if (CurrentArtifact != null)
        {
            var destinationPath = await GetDestinationPathAsync(new List<FsArtifact> { CurrentArtifact });
            var destinationDirectory = destinationPath ?? CurrentArtifact?.FullPath;

            if (CurrentArtifact != null && destinationPath != null)
            {
                if (_folderNameInputModalRef is null)
                {
                    return;
                }

                var folderName = Path.GetFileNameWithoutExtension(CurrentArtifact.Name);
                var createFolder = Localizer.GetString(AppStrings.FolderName);
                var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
                var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

                var folderNameResult = await _folderNameInputModalRef.ShowAsync(createFolder, string.Empty, folderName,
                    newFolderPlaceholder, extractBtnTitle);

                if (folderNameResult.ResultType == InputModalResultType.Cancel)
                {
                    SetGoBackDeviceButtonFunctionality();
                    return;
                }

                var destinationFolderName = folderNameResult.Result ?? folderName;

                if (folderNameResult.Result != null && _extractorModalRef != null)
                {
                    ExtractorBottomSheetResult = await _extractorModalRef.ShowAsync(CurrentArtifact.FullPath,
                        destinationPath,
                        folderNameResult.Result);
                }

                switch (ExtractorBottomSheetResult?.ExtractorResult)
                {
                    case ExtractorBottomSheetResultType.Cancel:
                        SetGoBackDeviceButtonFunctionality();
                        return;
                    case ExtractorBottomSheetResultType.Success when destinationDirectory != null:
                        {
                            var destinationResultPath = Path.Combine(destinationDirectory, destinationFolderName);
                            await NavigationFolderCallback.InvokeAsync(destinationResultPath);
                            break;
                        }
                    case null:
                        SetGoBackDeviceButtonFunctionality();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
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
        {
            SetGoBackDeviceButtonFunctionality();
            return null;
        }

        var destinationPath = result.SelectedArtifacts.FirstOrDefault()?.FullPath;
        return destinationPath;
    }

    private async Task HandleBackAsync(bool shouldExit = false)
    {
        switch (ArtifactExplorerMode)
        {
            case ArtifactExplorerMode.Normal when _currentInnerZipArtifact.FullPath == string.Empty || shouldExit:
                _cancellationTokenSource.Cancel();
                await OnBack.InvokeAsync();
                break;
            case ArtifactExplorerMode.Normal:
                _currentInnerZipArtifact = GetParent(_currentInnerZipArtifact);
                DisplayChildrenArtifacts(_currentInnerZipArtifact);
                break;
            case ArtifactExplorerMode.SelectArtifact:
                CancelSelectionMode();
                break;
            case ArtifactExplorerMode.SelectDestionation:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

    private void SetGoBackDeviceButtonFunctionality()
    {
        GoBackService.OnInit((async Task () =>
        {
            await HandleBackAsync();
            await Task.CompletedTask;
        }), true, false);
    }
}