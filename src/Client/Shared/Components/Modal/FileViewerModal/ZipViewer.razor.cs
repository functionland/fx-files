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
    private bool _isGoingBack;
    private ArtifactExplorer? _artifactExplorerRef;

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
            await LoadZipArtifactsAsync();
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isGoingBack)
        {
            _isGoingBack = false;
            await JSRuntime.InvokeVoidAsync("getLastScrollPosition", _artifactExplorerRef?.ArtifactExplorerListRef);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadZipArtifactsAsync()
    {
        try
        {
            if (CurrentArtifact is null)
                throw new InvalidOperationException("Current artifact can not be null.");

            var token = _cancellationTokenSource.Token;

            _allZipFileEntities =
                await _zipService.GetAllArtifactsAsync(CurrentArtifact.FullPath, cancellationToken: token);

            DisplayChildrenArtifacts(_currentInnerZipArtifact);
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
            await HandleBackAsync(true);
        }
        finally
        {
            await InvokeAsync(() =>
            {
                _isZipViewerInLoading = false;
                StateHasChanged();
            });
        }
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
        var destinationPath = await ShowDestinationSelectorModalAsync(artifacts);
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
        var destinationPath = await ShowDestinationSelectorModalAsync(new List<FsArtifact> { artifact });
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
            var destinationPath = await ShowDestinationSelectorModalAsync(new List<FsArtifact> { CurrentArtifact });
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

    private async Task<string?> ShowDestinationSelectorModalAsync(List<FsArtifact> artifacts)
    {
        if (_artifactSelectionModalRef is null)
            return null;

        var initialArtifactPath = CurrentArtifact?.ParentFullPath;

        var initialArtifact = string.IsNullOrWhiteSpace(initialArtifactPath)
            ? null
            : await FileService.GetArtifactAsync(initialArtifactPath);

        var result = await _artifactSelectionModalRef.ShowAsync(initialArtifact, AppStrings.ExtractHere, artifacts);
        
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
                await JSRuntime.InvokeVoidAsync("OnScrollEvent", _artifactExplorerRef?.ArtifactExplorerListRef);
                _isGoingBack = true;
                break;
            case ArtifactExplorerMode.SelectArtifact:
                CancelSelectionMode();
                break;
            case ArtifactExplorerMode.SelectDestination:
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

    private async Task HandleArtifactClickAsync(FsArtifact artifact)
    {
        if (artifact.ArtifactType != FsArtifactType.Folder)
            return;

        await JSRuntime.InvokeVoidAsync("saveScrollPosition", _artifactExplorerRef?.ArtifactExplorerListRef);
        _isGoingBack = false;
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
        GoBackService.SetState((async Task () =>
        {
            await HandleBackAsync();
            await Task.CompletedTask;
        }), true, false);
    }
}