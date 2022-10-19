using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;
    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _filteredArtifacts = new();
    private List<FsArtifact> _selectedArtifacts { get; set; } = new();

    private InputModal? _inputModalRef;
    private ConfirmationModal? _confirmationModalRef;
    private FilterArtifactModal? _filteredArtifactModalRef;
    private SortArtifactModal? _sortedArtifactModalRef;
    private ArtifactOverflowModal? _artifactOverflowModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private ConfirmationReplaceOrSkipModal? _confirmationReplaceOrSkipModalRef;
    private ArtifactDetailModal? _artifactDetailModalRef;
    private ProgressModal? _progressModalRef;
    private FxSearchInput? _fxSearchInputRef;

    private ArtifactActionResult _artifactActionResult { get; set; } = new();

    // ProgressBar
    private string ProgressBarCurrentText { get; set; } = default!;
    private string ProgressBarCurrentSubText { get; set; } = default!;
    private int ProgressBarCurrentValue { get; set; }
    private int ProgressBarMax { get; set; }
    private CancellationTokenSource? ProgressBarCts;
    private void ProgressBarOnCancel()
    {
        ProgressBarCts?.Cancel();
    }

    private string? _searchText;
    private bool _isInSearchMode;
    private FileCategoryType? _fileCategoryFilter;

    private ArtifactExplorerMode _artifactExplorerModeValue;
    private ArtifactExplorerMode _artifactExplorerMode
    {
        get { return _artifactExplorerModeValue; }
        set
        {
            if (_artifactExplorerModeValue != value)
            {
                ArtifactExplorerModeChange(value);
            }
        }
    }

    private SortTypeEnum _currentSortType = SortTypeEnum.Name;
    private bool _isAscOrder = true;
    private bool _isLoading = false;

    [Parameter] public IPinService PinService { get; set; } = default!;

    [Parameter] public IFileService FileService { get; set; } = default!;

    [Parameter] public ArtifactState ArtifactState { get; set; } = default!;

    protected override async Task OnInitAsync()
    {
        _isLoading = true;
        await LoadPinsAsync();
        await LoadChildrenArtifactsAsync();
        _isLoading = false;
        await base.OnInitAsync();
    }

    public async Task HandleCopyArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            List<FsArtifact> existArtifacts = new();
            var artifactActionResult = new ArtifactActionResult()
            {
                ActionType = ArtifactActionType.Copy,
                Artifacts = artifacts
            };

            string? destinationPath = await HandleSelectDestinationAsync(_currentArtifact, artifactActionResult);
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                return;
            }

            try
            {
                if (_progressModalRef is not null)
                {
                    await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.CopyFiles), true);
                }
                ProgressBarCts = new CancellationTokenSource();

                await FileService.CopyArtifactsAsync(artifacts, destinationPath, false
, onProgress: async (progressInfo) =>
                    {
                        ProgressBarCurrentText = progressInfo.CurrentText ?? String.Empty;
                        ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? String.Empty;
                        ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                        ProgressBarMax = progressInfo.MaxValue ?? artifacts.Count;
                        await InvokeAsync(() => StateHasChanged());
                    }, cancellationToken: ProgressBarCts.Token);

            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }
            finally
            {
                if (_progressModalRef is not null)
                {
                    await _progressModalRef.CloseAsync();
                }
            }

            if (_progressModalRef is not null)
            {
                await _progressModalRef.CloseAsync();
                StateHasChanged();
            }

            var overwriteArtifacts = GetShouldOverwriteArtiacts(artifacts, existArtifacts); //TODO: we must enhance this

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef != null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts.Count);

                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        ProgressBarCts = new CancellationTokenSource();

                        if (_progressModalRef is not null)
                        {
                            await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ReplacingFiles), true);

                            await FileService.CopyArtifactsAsync(overwriteArtifacts, destinationPath, true, onProgress: async (progressInfo) =>
                                {
                                    ProgressBarCurrentText = progressInfo.CurrentText ?? String.Empty;
                                    ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? String.Empty;
                                    ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                                    ProgressBarMax = progressInfo.MaxValue ?? artifacts.Count;
                                    await InvokeAsync(() => StateHasChanged());
                                },
                                cancellationToken: ProgressBarCts.Token);

                            await _progressModalRef.CloseAsync();
                        }
                    }
                    ChangeDeviceBackFunctionality(_artifactExplorerMode);
                }
            }

            var title = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedMessage);
            ToastModal.Show(title, message, FxToastType.Success);

            await NavigateToDestionation(destinationPath);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            if (_progressModalRef is not null)
            {
                await _progressModalRef.CloseAsync();
            }
        }
    }

    public async Task HandleMoveArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            List<FsArtifact> existArtifacts = new();
            var artifactActionResult = new ArtifactActionResult()
            {
                ActionType = ArtifactActionType.Move,
                Artifacts = artifacts
            };

            string? destinationPath = await HandleSelectDestinationAsync(_currentArtifact, artifactActionResult);
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                return;
            }

            try
            {
                ProgressBarCts = new CancellationTokenSource();

                if (_progressModalRef is not null)
                {
                    await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.MovingFiles), true);
                }

                await FileService.MoveArtifactsAsync(artifacts, destinationPath, false, onProgress: async (progressInfo) =>
                    {
                        ProgressBarCurrentText = progressInfo.CurrentText ?? String.Empty;
                        ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? String.Empty;
                        ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                        ProgressBarMax = progressInfo.MaxValue ?? artifacts.Count;
                        await InvokeAsync(() => StateHasChanged());
                    },
                    cancellationToken: ProgressBarCts.Token);
            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }
            finally
            {
                if (_progressModalRef is not null)
                {
                    await _progressModalRef.CloseAsync();
                }
            }

            var overwriteArtifacts = GetShouldOverwriteArtiacts(artifacts, existArtifacts); //TODO: we must enhance this

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef is not null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts.Count);
                    ChangeDeviceBackFunctionality(_artifactExplorerMode);

                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        ProgressBarCts = new CancellationTokenSource();
                        if (_progressModalRef is not null)
                        {
                            await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ReplacingFiles), true);
                        }

                        await FileService.MoveArtifactsAsync(overwriteArtifacts, destinationPath, true, onProgress: async (progressInfo) =>
                            {
                                ProgressBarCurrentText = progressInfo.CurrentText ?? String.Empty;
                                ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? String.Empty;
                                ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                                ProgressBarMax = progressInfo.MaxValue ?? artifacts.Count;
                                await InvokeAsync(() => StateHasChanged());
                            },
                            cancellationToken: ProgressBarCts.Token);
                    }
                }
            }

            _artifactExplorerMode = ArtifactExplorerMode.Normal;

            var title = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedMessage);
            ToastModal.Show(title, message, FxToastType.Success);

            await NavigateToDestionation(destinationPath);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            if (_progressModalRef is not null)
            {
                await _progressModalRef.CloseAsync();
            }
        }
    }

    public async Task<string?> HandleSelectDestinationAsync(FsArtifact? artifact, ArtifactActionResult artifactActionResult)
    {
        var result = await _artifactSelectionModalRef!.ShowAsync(artifact, artifactActionResult);
        ChangeDeviceBackFunctionality(_artifactExplorerMode);

        string? destinationPath = null;

        if (result?.ResultType == ArtifactSelectionResultType.Ok)
        {
            var destinationFsArtifact = result.SelectedArtifacts.FirstOrDefault();
            destinationPath = destinationFsArtifact?.FullPath;
        }

        return destinationPath;
    }

    public async Task HandleRenameArtifactAsync(FsArtifact? artifact)
    {
        var result = await GetInputModalResult(artifact);
        if (result?.ResultType == InputModalResultType.Cancel)
        {
            return;
        }

        string? newName = result?.ResultName;

        if (artifact?.ArtifactType == FsArtifactType.Folder)
        {
            await RenameFolderAsync(artifact, newName);
        }
        else if (artifact?.ArtifactType == FsArtifactType.File)
        {
            await RenameFileAsync(artifact, newName);
        }
        else if (artifact?.ArtifactType == FsArtifactType.Drive)
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.RootfolderRenameException);
            ToastModal.Show(title, message, FxToastType.Error);
        }
    }

    public async Task HandlePinArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            _isLoading = true;
            await PinService.SetArtifactsPinAsync(artifacts);
            await UpdatePinedArtifactsAsync(artifacts, true);
            _isLoading = false;
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    public async Task HandleUnPinArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            _isLoading = true;
            var pathArtifacts = artifacts.Select(a => a.FullPath);
            await PinService.SetArtifactsUnPinAsync(pathArtifacts);
            await UpdatePinedArtifactsAsync(artifacts, false);
            _isLoading = false;
        }
        catch (Exception exception)
        {
            _isLoading = false;
            ExceptionHandler?.Handle(exception);
        }
    }

    public async Task HandleDeleteArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            if (_confirmationModalRef != null)
            {
                var result = new ConfirmationModalResult();

                if (artifacts.Count == 1)
                {
                    var singleArtifact = artifacts.SingleOrDefault();
                    result = await _confirmationModalRef.ShowAsync(Localizer.GetString(AppStrings.DeleteItems, singleArtifact?.Name), Localizer.GetString(AppStrings.DeleteItemDescription));
                    ChangeDeviceBackFunctionality(_artifactExplorerMode);
                }
                else
                {
                    result = await _confirmationModalRef.ShowAsync(Localizer.GetString(AppStrings.DeleteItems, artifacts.Count), Localizer.GetString(AppStrings.DeleteItemsDescription));
                    ChangeDeviceBackFunctionality(_artifactExplorerMode);
                }

                if (result.ResultType == ConfirmationModalResultType.Confirm)
                {
                    ProgressBarCts = new CancellationTokenSource();
                    if (_progressModalRef is not null)
                    {
                        await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.DeletingFiles), true);

                        await FileService.DeleteArtifactsAsync(artifacts, onProgress: async (progressInfo) =>
                            {
                                ProgressBarCurrentText = progressInfo.CurrentText ?? String.Empty;
                                ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? String.Empty;
                                ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                                ProgressBarMax = progressInfo.MaxValue ?? artifacts.Count;
                                var deletedArtifact = artifacts.FirstOrDefault(a => a.Name == ProgressBarCurrentText);
                                if (deletedArtifact != null)
                                {
                                    await UpdateRemovedArtifactsAsync(deletedArtifact);
                                }
                                await InvokeAsync(() => StateHasChanged());
                            },
                            cancellationToken: ProgressBarCts.Token);

                        await _progressModalRef.CloseAsync();

                    }
                }
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
            _isLoading = false;
        }
    }

    public async Task HandleShowDetailsArtifact(List<FsArtifact> artifact)
    {
        var isMultiple = artifact.Count > 1 ? true : false;
        var result = await _artifactDetailModalRef!.ShowAsync(artifact, isMultiple);
        ChangeDeviceBackFunctionality(_artifactExplorerMode);

        switch (result.ResultType)
        {
            case ArtifactDetailModalResultType.Download:
                //TODO: Implement download logic here
                //await HandleDownloadArtifacts(artifact);
                break;
            case ArtifactDetailModalResultType.Move:
                await HandleMoveArtifactsAsync(artifact);
                break;
            case ArtifactDetailModalResultType.Pin:
                await HandlePinArtifactsAsync(artifact);
                break;
            case ArtifactDetailModalResultType.Unpin:
                await HandleUnPinArtifactsAsync(artifact);
                break;
            case ArtifactDetailModalResultType.More:
                if (artifact.Count > 1)
                {
                    await HandleSelectedArtifactsOptions(artifact);
                }
                else
                {
                    await HandleOptionsArtifact(artifact[0]);
                }
                break;
            case ArtifactDetailModalResultType.Upload:
                //TODO: Implement upload logic here
                break;
            case ArtifactDetailModalResultType.Close:
                break;
            default:
                break;
        }
    }

    public async Task HandleCreateFolder(string path)
    {
        if (_inputModalRef is null) return;

        var createFolder = Localizer.GetString(AppStrings.CreateFolder);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.NewFolderPlaceholder);

        var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, string.Empty, newFolderPlaceholder);
        ChangeDeviceBackFunctionality(_artifactExplorerMode);

        try
        {
            if (result?.ResultType == InputModalResultType.Confirm)
            {
                var newFolder = await FileService.CreateFolderAsync(path, result?.ResultName); //ToDo: Make CreateFolderAsync nullable
                _allArtifacts.Add(newFolder);   //Ugly, but no other possible way for now.
                FilterArtifacts();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private async Task LoadPinsAsync()
    {
        var allPins = await PinService.GetPinnedArtifactsAsync();

        var pins = new List<FsArtifact>();

        foreach (var item in allPins)
        {
            pins.Add(item);
        }

        _pins = pins;
    }

    private async Task LoadChildrenArtifactsAsync(FsArtifact? artifact = null)
    {
        try
        {
            if (artifact is null)
            {
                GoBackService.OnInit(null, true, true);
            }
            else
            {
                GoBackService.OnInit(HandleToolbarBackClick, true, false);
            }

            var allFiles = FileService.GetArtifactsAsync(artifact?.FullPath);
            var artifacts = new List<FsArtifact>();
            await foreach (var item in allFiles)
            {
                item.IsPinned = await PinService.IsPinnedAsync(item);
                artifacts.Add(item);
            }

            _allArtifacts = artifacts;
            FilterArtifacts();
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }

    private async Task HandleSelectArtifactAsync(FsArtifact artifact)
    {
        _fxSearchInputRef?.HandleClearInputText();
        if (artifact.ArtifactType == FsArtifactType.File)
        {
#if BlazorHybrid
            try
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(artifact.FullPath)
                });
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Handle(exception);
            }
#endif
        }
        else
        {
            _currentArtifact = artifact;
            _isLoading = true;
            await LoadChildrenArtifactsAsync(_currentArtifact);
            _isLoading = false;
        }
    }

    private async Task HandleOptionsArtifact(FsArtifact artifact)
    {
        ArtifactOverflowResult? result = null;
        if (_artifactOverflowModalRef is not null)
        {
            var pinOptionResult = new PinOptionResult()
            {
                IsVisible = true,
                Type = artifact.IsPinned == true ? PinOptionResultType.Remove : PinOptionResultType.Add
            };
            result = await _artifactOverflowModalRef!.ShowAsync(false, pinOptionResult);
            ChangeDeviceBackFunctionality(_artifactExplorerMode);
        }

        switch (result?.ResultType)
        {
            case ArtifactOverflowResultType.Details:
                _isLoading = true;
                await HandleShowDetailsArtifact(new List<FsArtifact> { artifact });
                _isLoading = false;
                break;
            case ArtifactOverflowResultType.Rename:
                await HandleRenameArtifactAsync(artifact);
                break;
            case ArtifactOverflowResultType.Copy:
                await HandleCopyArtifactsAsync(new List<FsArtifact> { artifact });
                break;
            case ArtifactOverflowResultType.Pin:
                await HandlePinArtifactsAsync(new List<FsArtifact> { artifact });
                break;
            case ArtifactOverflowResultType.UnPin:
                await HandleUnPinArtifactsAsync(new List<FsArtifact> { artifact });
                break;
            case ArtifactOverflowResultType.Move:
                await HandleMoveArtifactsAsync(new List<FsArtifact> { artifact });
                break;
            case ArtifactOverflowResultType.Delete:
                await HandleDeleteArtifactsAsync(new List<FsArtifact> { artifact });
                break;
        }
    }

    public void ToggleSelectedAll()
    {
        if (_artifactExplorerMode == ArtifactExplorerMode.Normal)
        {
            _artifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
            _selectedArtifacts = new List<FsArtifact>();
            foreach (var artifact in _allArtifacts)
            {
                artifact.IsSelected = true;
                _selectedArtifacts.Add(artifact);
            }
        }
    }

    public void ChangeViewMode(ViewModeEnum viewMode)
    {
        ArtifactState.SetViewMode(viewMode);
        StateHasChanged();
    }

    public void CancelSelectionMode()
    {
        _artifactExplorerMode = ArtifactExplorerMode.Normal;
        foreach (var artifact in _selectedArtifacts)
        {
            artifact.IsSelected = false;
        }
        _selectedArtifacts.Clear();
    }

    private async Task HandleSelectedArtifactsOptions(List<FsArtifact> artifacts)
    {
        var selectedArtifactsCount = artifacts.Count;
        var isMultiple = selectedArtifactsCount > 1;

        if (selectedArtifactsCount > 0)
        {
            ArtifactOverflowResult? result = null;
            if (_artifactOverflowModalRef is not null)
            {
                _artifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
                var pinOptionResult = GetPinOptionResult(artifacts);
                result = await _artifactOverflowModalRef!.ShowAsync(isMultiple, pinOptionResult);
                ChangeDeviceBackFunctionality(_artifactExplorerMode);
            }

            switch (result?.ResultType)
            {
                case ArtifactOverflowResultType.Details:
                    _isLoading = true;
                    await HandleShowDetailsArtifact(artifacts);
                    _isLoading = false;
                    break;
                case ArtifactOverflowResultType.Rename when (!isMultiple):
                    var singleArtifact = artifacts.SingleOrDefault();
                    await HandleRenameArtifactAsync(singleArtifact);
                    break;
                case ArtifactOverflowResultType.Copy:
                    await HandleCopyArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Pin:
                    await HandlePinArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.UnPin:
                    await HandleUnPinArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Move:
                    await HandleMoveArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Delete:
                    await HandleDeleteArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Cancel:
                    _artifactExplorerMode = ArtifactExplorerMode.Normal;
                    break;
            }

            _artifactExplorerMode = ArtifactExplorerMode.Normal;
        }
    }

    private void ArtifactExplorerModeChange(ArtifactExplorerMode mode)
    {
        ChangeDeviceBackFunctionality(mode);
        _artifactExplorerModeValue = mode;

        if (mode == ArtifactExplorerMode.Normal)
        {
            CancelSelectionMode();
        }

        StateHasChanged();
    }

    private PinOptionResult GetPinOptionResult(List<FsArtifact> artifacts)
    {
        if (artifacts.All(a => a.IsPinned == true))
        {
            return new PinOptionResult()
            {
                IsVisible = true,
                Type = PinOptionResultType.Remove
            };
        }
        else if (artifacts.All(a => a.IsPinned == false))
        {
            return new PinOptionResult()
            {
                IsVisible = true,
                Type = PinOptionResultType.Add
            };
        }

        return new PinOptionResult()
        {
            IsVisible = false,
            Type = null
        };
    }

    private async Task<InputModalResult?> GetInputModalResult(FsArtifact? artifact)
    {
        string artifactType = "";

        if (artifact?.ArtifactType == FsArtifactType.File)
        {
            artifactType = Localizer.GetString(AppStrings.FileRenamePlaceholder);
        }
        else if (artifact?.ArtifactType == FsArtifactType.Folder)
        {
            artifactType = Localizer.GetString(AppStrings.FolderRenamePlaceholder);
        }
        else
        {
            return null;
        }

        var Name = Path.GetFileNameWithoutExtension(artifact.Name);

        InputModalResult? result = null;
        if (_inputModalRef is not null)
        {
            result = await _inputModalRef.ShowAsync(Localizer.GetString(AppStrings.ChangeName), Localizer.GetString(AppStrings.Rename).ToString().ToUpper(), Name, artifactType);
            ChangeDeviceBackFunctionality(_artifactExplorerMode);
        }

        return result;
    }

    private void UpdateRenamedArtifact(FsArtifact artifact, string fullNewName)
    {
        FsArtifact? artifactRenamed = null;

        if (artifact.FullPath == _currentArtifact?.FullPath)
        {
            artifactRenamed = _currentArtifact;
        }
        else
        {
            artifactRenamed = _allArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
        }

        if (artifactRenamed != null)
        {
            var artifactParentPath = Path.GetDirectoryName(artifact.FullPath) ?? "";
            artifactRenamed.FullPath = Path.Combine(artifactParentPath, fullNewName);
            artifactRenamed.Name = fullNewName;
            FilterArtifacts();
        }
    }

    private async Task UpdatePinedArtifactsAsync(IEnumerable<FsArtifact> artifacts, bool IsPinned)
    {
        await LoadPinsAsync();
        var artifactPath = artifacts.Select(a => a.FullPath);

        if (_currentArtifact != null && artifactPath.Any(p => p == _currentArtifact.FullPath))
        {
            _currentArtifact.IsPinned = IsPinned;
        }
        else
        {
            foreach (var artifact in _allArtifacts)
            {
                if (artifactPath.Contains(artifact.FullPath))
                {
                    artifact.IsPinned = IsPinned;
                }
            }
            FilterArtifacts();
        }
    }

    private async Task UpdateRemovedArtifactsAsync(FsArtifact artifact)
    {
        if (artifact.FullPath == _currentArtifact?.FullPath)
        {
            await HandleToolbarBackClick();
            return;
        }
        _allArtifacts.Remove(artifact);
        FilterArtifacts();
    }

    private async Task HandleCancelSearchAsync()
    {
        _isLoading = true;
        _isInSearchMode = false;
        cancellationTokenSource?.Cancel();
        _searchText = string.Empty;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        _isLoading = false;
    }

    private void HandleSearchFocused()
    {
        _isInSearchMode = true;
    }

    CancellationTokenSource? cancellationTokenSource;

    private async Task HandleDeepSearchAsync(string? text)
    {
        _isLoading = true;
        _searchText = text;
        _allArtifacts.Clear();
        _filteredArtifacts.Clear();

        FilterArtifacts();

        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
        }

        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        await Task.Run(async () =>
        {
            var buffer = new List<FsArtifact>();
            try
            {
                await foreach (var item in FileService.GetArtifactsAsync(_currentArtifact?.FullPath, _searchText, token))
                {
                    if (token.IsCancellationRequested)
                        return;

                    _allArtifacts.Add(item);
                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        FilterArtifacts();
                        await InvokeAsync(() =>
                        {
                            if (_isLoading)
                            {
                                _isLoading = false;
                            }
                            StateHasChanged();
                        });
                        sw.Restart();
                        await Task.Yield();
                    }
                }

                if (token.IsCancellationRequested)
                    return;

                FilterArtifacts();
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            finally
            {
                _isLoading = false;
            }

        });
    }

    private void HandleSearch(string? text)
    {
        if (text != null)
        {
            _searchText = text;
            FilterArtifacts();
        }
    }

    private async Task HandleToolbarBackClick()
    {
        _searchText = string.Empty;
        _fxSearchInputRef?.HandleClearInputText();

        if (_artifactExplorerMode != ArtifactExplorerMode.Normal)
        {
            _artifactExplorerMode = ArtifactExplorerMode.Normal;
        }
        else
        {
            if (!_isInSearchMode)
            {
                _fxSearchInputRef?.HandleClearInputText();
                await UpdateCurrentArtifactForBackButton(_currentArtifact);
                await LoadChildrenArtifactsAsync(_currentArtifact);
                await JSRuntime.InvokeVoidAsync("OnScrollEvent");
                StateHasChanged();
            }
            else
            {
                cancellationTokenSource?.Cancel();
                _isInSearchMode = false;
                _fxSearchInputRef?.HandleClearInputText();
                await LoadChildrenArtifactsAsync();
                StateHasChanged();
            }
        }
    }

    private async Task UpdateCurrentArtifactForBackButton(FsArtifact? fsArtifact)
    {
        try
        {
            _currentArtifact = await FileService.GetArtifactAsync(fsArtifact?.ParentFullPath);
        }
        catch (DomainLogicException ex) when (ex is ArtifactPathNullException)
        {
            _currentArtifact = null;
        }
    }

    private void FilterArtifacts()
    {
        _filteredArtifacts = string.IsNullOrWhiteSpace(_searchText)
            ? _allArtifacts
            : _allArtifacts.Where(a => a.Name.ToLower().Contains(_searchText.ToLower())).ToList();

        _filteredArtifacts = _fileCategoryFilter is null
            ? _filteredArtifacts
            : _filteredArtifacts.Where(fa =>
            {
                if (_fileCategoryFilter == FileCategoryType.Document)
                {
                    return (fa.FileCategory == FileCategoryType.Document
                                                || fa.FileCategory == FileCategoryType.Pdf
                                                || fa.FileCategory == FileCategoryType.Other);
                }
                return fa.FileCategory == _fileCategoryFilter;
            }).ToList();
    }

    private async Task HandleFilterClick()
    {
        _fileCategoryFilter = await _filteredArtifactModalRef!.ShowAsync();
        ChangeDeviceBackFunctionality(_artifactExplorerMode);
        await JSRuntime.InvokeVoidAsync("OnScrollEvent");
        FilterArtifacts();
    }

    private void HandleSortOrderClick()
    {
        _isAscOrder = !_isAscOrder;
        SortFilteredArtifacts();
    }

    private async Task HandleSortClick()
    {
        _currentSortType = await _sortedArtifactModalRef!.ShowAsync();
        ChangeDeviceBackFunctionality(_artifactExplorerMode);
        SortFilteredArtifacts();
    }

    private void SortFilteredArtifacts()
    {
        if (_currentSortType is SortTypeEnum.LastModified)
        {
            if (_isAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.LastModifiedDateTime).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.LastModifiedDateTime).ToList();
                return;
            }

        }

        if (_currentSortType is SortTypeEnum.Size)
        {
            if (_isAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Size).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.Size).ToList();
                return;
            }
        }

        if (_currentSortType is SortTypeEnum.Name)
        {
            if (_isAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Name).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.Name).ToList();
                return;
            }
        }
    }

    private async Task RenameFileAsync(FsArtifact? artifact, string? newName)
    {
        try
        {
            await FileService.RenameFileAsync(artifact.FullPath, newName);
            var fullName = newName + artifact.FileExtension;
            var artifactRenamed = _allArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
            UpdateRenamedArtifact(artifact, fullName);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private async Task RenameFolderAsync(FsArtifact? artifact, string? newName)
    {
        try
        {
            await FileService.RenameFolderAsync(artifact.FullPath, newName);
            UpdateRenamedArtifact(artifact, newName);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private static List<FsArtifact> GetShouldOverwriteArtiacts(List<FsArtifact> artifacts, List<FsArtifact> existArtifacts)
    {
        List<FsArtifact> overwriteArtifacts = new();
        var pathExistArtifacts = existArtifacts.Select(a => a.FullPath);
        foreach (var artifact in artifacts)
        {
            if (pathExistArtifacts.Any(p => p.StartsWith(artifact.FullPath)))
            {
                overwriteArtifacts.Add(artifact);
            }
        }

        return overwriteArtifacts;
    }

    private async Task NavigateToDestionation(string? destinationPath)
    {
        _currentArtifact = await FileService.GetArtifactAsync(destinationPath);
        _isLoading = true;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        await LoadPinsAsync();
        _isLoading = false;
    }

    private void ChangeDeviceBackFunctionality(ArtifactExplorerMode mode)
    {
        if (mode == ArtifactExplorerMode.SelectArtifact)
        {
            GoBackService.OnInit((Task () =>
            {
                CancelSelectionMode();
                return Task.CompletedTask;
            }), true, false);
        }
        else if (mode == ArtifactExplorerMode.Normal)
        {
            if (_currentArtifact == null)
            {
                GoBackService.OnInit(null, true, true);
            }
            else
            {
                GoBackService.OnInit((Task () =>
                {
                    CancelSelectionMode();
                    return Task.CompletedTask;
                }), true, false);
            }

        }
    }
}