﻿using System.Net;

using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser
{
    // Modals
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
    private FileViewer? _fileViewerRef;

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

    private FsArtifact? _currentArtifact;
    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();
    private string _inlineSearchText = string.Empty;
    private string _searchText = string.Empty;
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
    private bool _isArtifactExplorerLoading = false;
    private bool _isPinBoxLoading = true;
    private bool _isGoingBack;

    [Parameter] public IPinService PinService { get; set; } = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;

    [Parameter] public InMemoryAppStateStore ArtifactState { get; set; } = default!;
    [Parameter] public IViewFileService<IFileService> ViewFileService { get; set; } = default!;
    [Parameter] public string? DefaultPath { get; set; }


    protected override async Task OnInitAsync()
    {

        Task PinTask = LoadPinsAsync();
        Task ArtifactListTask;

        if (string.IsNullOrWhiteSpace(DefaultPath))
        {
            ArtifactListTask = LoadChildrenArtifactsAsync();
        }
        else
        {
            var defaultArtifact = await FileService.GetArtifactAsync(DefaultPath);
            _currentArtifact = defaultArtifact;
            ArtifactListTask = LoadChildrenArtifactsAsync(defaultArtifact);
        }

        await base.OnInitAsync();

    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isGoingBack)
        {
            _isGoingBack = false;
            await JSRuntime.InvokeVoidAsync("getLastScrollPossition");
        }
        await base.OnAfterRenderAsync(firstRender);
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

                await FileService.CopyArtifactsAsync(artifacts, destinationPath, false,
                    onProgress: async (progressInfo) =>
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

                            await FileService.CopyArtifactsAsync(overwriteArtifacts, destinationPath, true,
                                onProgress: async (progressInfo) =>
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
            _isPinBoxLoading = true;
            await PinService.SetArtifactsPinAsync(artifacts);
            await UpdatePinedArtifactsAsync(artifacts, true);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            _isPinBoxLoading = false;
        }
    }

    public async Task HandleUnPinArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            _isPinBoxLoading = true;
            var pathArtifacts = artifacts.Select(a => a.FullPath);
            await PinService.SetArtifactsUnPinAsync(pathArtifacts);
            await UpdatePinedArtifactsAsync(artifacts, false);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
            _isPinBoxLoading = false;
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
        }
    }

    public async Task HandleShowDetailsArtifact(List<FsArtifact> artifact)
    {
        bool isMultiple = artifact.Count > 1 ? true : false;
        bool isDrive = false;

        if (isMultiple is false)
        {
            isDrive = artifact.SingleOrDefault()?.ArtifactType == FsArtifactType.Drive;
        }

        var result = await _artifactDetailModalRef!.ShowAsync(artifact, isMultiple, (isDrive || IsInRoot(_currentArtifact)));
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
                RefreshDisplayedArtifacts();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    public async Task HandleShareFiles(List<FsArtifact> artifacts)
    {
        _isArtifactExplorerLoading = true;
        StateHasChanged();
        var files = GetShareFiles(artifacts);

        await Share.Default.RequestAsync(new ShareMultipleFilesRequest
        {
            Title = "Share with app",
            Files = files
        });
        _isArtifactExplorerLoading = false;
    }

    private List<ShareFile> GetShareFiles(List<FsArtifact> artifacts)
    {
        var files = new List<ShareFile>();
        foreach (var artifact in artifacts)
        {
            if (artifact.ArtifactType == FsArtifactType.File)
            {
                files.Add(new ShareFile(artifact.FullPath));
            }
        }

        return files;
    }

    private async Task LoadPinsAsync()
    {
        _isPinBoxLoading = true;
        try
        {
            _pins = await PinService.GetPinnedArtifactsAsync();
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            _isPinBoxLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadChildrenArtifactsAsync(FsArtifact? artifact = null)
    {
        _isArtifactExplorerLoading = true;

        try
        {
            var childrenArtifacts = FileService.GetArtifactsAsync(artifact?.FullPath);
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
            await foreach (var item in childrenArtifacts)
            {
                item.IsPinned = await PinService.IsPinnedAsync(item);
                artifacts.Add(item);
            }

            _allArtifacts = artifacts;
            RefreshDisplayedArtifacts();
        }
        catch (ArtifactUnauthorizedAccessException exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            _isArtifactExplorerLoading = false;
            StateHasChanged();
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
            var isOpened = _fileViewerRef?.OpenArtifact(artifact);

            if (isOpened == false)
            {
#if BlazorHybrid
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(artifact?.FullPath)
                    });
                }
                catch (UnauthorizedAccessException)
                {
                    ExceptionHandler?.Handle(new DomainLogicException(Localizer.GetString(nameof(AppStrings.ArtifactUnauthorizedAccessException))));;
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Handle(exception);
                }
#endif
            }
        }
        else
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("saveScrollPosition");
                _isGoingBack = false;
                _currentArtifact = artifact;
                _isArtifactExplorerLoading = true;
                await LoadChildrenArtifactsAsync(_currentArtifact);
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Handle(exception);
            }
            finally
            {
                _isArtifactExplorerLoading = false;
            }
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
            var isDrive = artifact?.ArtifactType == FsArtifactType.Drive;
            var isVisibleShareWithApp = artifact?.ArtifactType == FsArtifactType.File;
            result = await _artifactOverflowModalRef!.ShowAsync(false, pinOptionResult, isVisibleShareWithApp, isDrive);
            ChangeDeviceBackFunctionality(_artifactExplorerMode);
        }

        switch (result?.ResultType)
        {
            case ArtifactOverflowResultType.Details:
                try
                {
                    _isArtifactExplorerLoading = true;
                    await HandleShowDetailsArtifact(new List<FsArtifact> { artifact });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Handle(exception);
                }
                finally
                {
                    _isArtifactExplorerLoading = false;
                }
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
            case ArtifactOverflowResultType.ShareWithApp:
                await HandleShareFiles(new List<FsArtifact> { artifact });
                break;
            case ArtifactOverflowResultType.Delete:
                await HandleDeleteArtifactsAsync(new List<FsArtifact> { artifact });
                break;
        }
    }

    public async Task ToggleSelectedAll()
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

    public void ChangeViewMode()
    {
        var viewMode = ArtifactState.ViewMode == ViewModeEnum.List ? ViewModeEnum.Grid : ViewModeEnum.List;
        ArtifactState.SetViewMode(viewMode);
        StateHasChanged();
    }

    public void CancelSelectionMode()
    {
        foreach (var artifact in _selectedArtifacts)
        {
            artifact.IsSelected = false;
        }
        _selectedArtifacts.Clear();
        _artifactExplorerMode = ArtifactExplorerMode.Normal;
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
                var isVisibleSahreWithApp = !artifacts.Any(a => a.ArtifactType != FsArtifactType.File);
                result = await _artifactOverflowModalRef!.ShowAsync(isMultiple, pinOptionResult, isVisibleSahreWithApp, IsInRoot(_currentArtifact));
                ChangeDeviceBackFunctionality(_artifactExplorerMode);
            }

            switch (result?.ResultType)
            {
                case ArtifactOverflowResultType.Details:
                    try
                    {
                        _isArtifactExplorerLoading = true;
                        await HandleShowDetailsArtifact(artifacts);
                    }
                    catch (Exception exception)
                    {
                        ExceptionHandler?.Handle(exception);
                    }
                    finally
                    {
                        _isArtifactExplorerLoading = false;
                    }
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
                case ArtifactOverflowResultType.ShareWithApp:
                    await HandleShareFiles(artifacts);
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

    private async Task<string?> HandleSelectDestinationAsync(FsArtifact? artifact, ArtifactActionResult artifactActionResult)
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
            RefreshDisplayedArtifacts();
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
            RefreshDisplayedArtifacts();
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
        RefreshDisplayedArtifacts();
    }

    private async Task HandleCancelCurrentListSearchAsync()
    {
        //_isLoading = true;
        _isInSearchMode = false;
        cancellationTokenSource?.Cancel();
        _inlineSearchText = string.Empty;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        //_isLoading = false;
    }

    private void HandleDeepSearchFocused()
    {
        _isInSearchMode = true;
    }

    CancellationTokenSource? cancellationTokenSource;

    private async Task HandleDeepSearchAsync(string text)
    {
        //_isLoading = true;
        _searchText = text;
        _allArtifacts.Clear();
        _displayedArtifacts.Clear();

        RefreshDisplayedArtifacts();

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

                        RefreshDisplayedArtifacts();
                        await InvokeAsync(() =>
                        {
                            //if (_isLoading)
                            //{
                            //    _isLoading = false;
                            //}
                            StateHasChanged();
                        });
                        sw.Restart();
                        await Task.Yield();
                    }
                }

                if (token.IsCancellationRequested)
                    return;

                RefreshDisplayedArtifacts();
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
                //_isLoading = false;
            }

        });
    }

    private void HandleCurrentListSearch(string text)
    {
        if (text != null)
        {
            _inlineSearchText = text;
            RefreshDisplayedArtifacts();
        }
    }

    private async Task HandleToolbarBackClick()
    {
        _searchText = string.Empty;
        _inlineSearchText = string.Empty;
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
                _isGoingBack = true;
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

    private void RefreshDisplayedArtifacts(
        bool applyInlineSearch = true,
        bool applyFilters = true,
        bool applySort = true)
    {
        IEnumerable<FsArtifact> displayingArtifacts = _allArtifacts;

        if (applyInlineSearch)
        {
            displayingArtifacts = ApplyInlineSearch(displayingArtifacts);
        }

        if (applyFilters)
        {
            displayingArtifacts = ApplyFilters(displayingArtifacts);
        }

        if (applySort)
        {
            displayingArtifacts = ApplySort(displayingArtifacts);
        }

        _displayedArtifacts = displayingArtifacts.ToList();

    }

    private IEnumerable<FsArtifact> ApplyInlineSearch(IEnumerable<FsArtifact> artifacts)
    {
        return (string.IsNullOrEmpty(_inlineSearchText) || string.IsNullOrWhiteSpace(_inlineSearchText))
            ? artifacts
            : artifacts.Where(a => a.Name.ToLower().Contains(_inlineSearchText.ToLower()));
    }

    private IEnumerable<FsArtifact> ApplyFilters(IEnumerable<FsArtifact> artifacts)
    {
        return _fileCategoryFilter is null
            ? artifacts
            : artifacts.Where(fa =>
            {
                if (_fileCategoryFilter == FileCategoryType.Document)
                {
                    return (fa.FileCategory == FileCategoryType.Document
                                                || fa.FileCategory == FileCategoryType.Pdf
                                                || fa.FileCategory == FileCategoryType.Other);
                }
                return fa.FileCategory == _fileCategoryFilter;
            });
    }

    private IEnumerable<FsArtifact> ApplySort(IEnumerable<FsArtifact> artifacts)
    {
        return SortDisplayedArtifacts(artifacts);
    }

    private async Task HandleFilterClick()
    {
        if (_isArtifactExplorerLoading) return;

        _fileCategoryFilter = await _filteredArtifactModalRef!.ShowAsync();
        ChangeDeviceBackFunctionality(_artifactExplorerMode);
        await JSRuntime.InvokeVoidAsync("OnScrollEvent");
        _isArtifactExplorerLoading = true;
        await Task.Run(() =>
        {
            RefreshDisplayedArtifacts();
        });
        _isArtifactExplorerLoading = false;
  
    }

    private async Task HandleSortOrderClick()
    {
        if (_isArtifactExplorerLoading) return;
        
        _isAscOrder = !_isAscOrder;
        _isArtifactExplorerLoading = true;
        try
        {
            await Task.Run(() =>
            {
                var sortedDisplayArtifact = SortDisplayedArtifacts(_displayedArtifacts);
                _displayedArtifacts = sortedDisplayArtifact.ToList();
            });
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            _isArtifactExplorerLoading = false;
        }
    }

    private async Task HandleSortClick()
    {
        if (_isArtifactExplorerLoading) return;

        _currentSortType = await _sortedArtifactModalRef!.ShowAsync();
        ChangeDeviceBackFunctionality(_artifactExplorerMode);
        _isArtifactExplorerLoading = true;
        StateHasChanged();
        try
        {
            var sortedDisplayArtifact = SortDisplayedArtifacts(_displayedArtifacts);
            _displayedArtifacts = sortedDisplayArtifact.ToList();
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            _isArtifactExplorerLoading = false;
        }
    }

    private IEnumerable<FsArtifact> SortDisplayedArtifacts(IEnumerable<FsArtifact> artifacts)
    {
        IEnumerable<FsArtifact> sortedArtifactsQuery;
        if (_currentSortType is SortTypeEnum.LastModified)
        {
            if (_isAscOrder)
            {
                sortedArtifactsQuery = artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.LastModifiedDateTime);
            }
            else
            {
                sortedArtifactsQuery = artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.LastModifiedDateTime);
            }

        }

        else if (_currentSortType is SortTypeEnum.Size)
        {
            if (_isAscOrder)
            {
                sortedArtifactsQuery = artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Size);
            }
            else
            {
                sortedArtifactsQuery = artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.Size);
            }
        }

        else if (_currentSortType is SortTypeEnum.Name)
        {
            if (_isAscOrder)
            {
                sortedArtifactsQuery = artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Name);
            }
            else
            {
                sortedArtifactsQuery = artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder).ThenByDescending(artifact => artifact.Name);
            }
        }
        else
        {
            sortedArtifactsQuery = artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Name);
        }

        return sortedArtifactsQuery;
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
        //_isLoading = true;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        await LoadPinsAsync();
        //_isLoading = false;
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