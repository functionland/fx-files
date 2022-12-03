﻿using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Functionland.FxFiles.Client.Shared.Utils;
using Prism.Events;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser : IDisposable
{
    // Modals
    private InputModal? _inputModalRef;
    private InputModal? _passwordModalRef;
    private ConfirmationModal? _confirmationModalRef;
    private FilterArtifactModal? _filteredArtifactModalRef;
    private SortArtifactModal? _sortedArtifactModalRef;
    private ArtifactOverflowModal? _artifactOverflowModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private ConfirmationReplaceOrSkipModal? _confirmationReplaceOrSkipModalRef;
    private ArtifactDetailModal? _artifactDetailModalRef;
    private ProgressModal? _progressModalRef;


    private FxBreadcrumbs? _breadcrumbsRef;
    private FxSearchInput? _fxSearchInputRef;
    private FileViewer? _fileViewerRef;
    private ExtractorBottomSheet? _extractorModalRef;
    private FxSearchInput? _searchInputRef;
    private FxToolBar? _fxToolBarRef;
    private ArtifactExplorer? _artifactExplorerRef;

    // ProgressBar
    private string ProgressBarCurrentText { get; set; } = default!;
    private string ProgressBarCurrentSubText { get; set; } = default!;
    private double ProgressBarCurrentValue { get; set; }
    private int ProgressBarMax { get; set; }

    private CancellationTokenSource? ProgressBarCts
    {
        get => _progressBarCts;
        set
        {
            _progressBarCts?.Dispose();
            _progressBarCts = value;
        }
    }

    private CancellationTokenSource? _searchCancellationTokenSource;
    private CancellationTokenSource? _progressBarCts;

    // Search
    private bool _isFileCategoryFilterBoxOpen = true;
    private bool _isInSearchMode;
    private bool _isSearchInputFocused = false;
    private string? _inlineSearchText = string.Empty;
    private string _searchText = string.Empty;
    private ArtifactDateSearchType? _artifactsSearchFilterDate;
    private List<ArtifactCategorySearchType> _artifactsSearchFilterTypes = new();
    private PinOptionResult? _searchPinOptionResult;

    private FsArtifact? _currentArtifactValue;
    private FsArtifact? CurrentArtifact
    {
        get => _currentArtifactValue;
        set
        {
            if (_currentArtifactValue == value)
                return;

            if (_currentArtifactValue is not null)
            {
                FileWatchService.UnWatchArtifact(_currentArtifactValue);
            }

            _currentArtifactValue = value;

            if (_currentArtifactValue is not null)
            {
                FileWatchService.WatchArtifact(_currentArtifactValue);
            }

            AppStateStore.CurrentMyDeviceArtifact = value;
        }
    }

    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _searchResultArtifacts = new();
    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();

    private FileCategoryType? _inlineFileCategoryFilter;
    private FileCategoryType? InlineFileCategoryFilter
    {
        get => _inlineFileCategoryFilter;
        set
        {
            if (_inlineFileCategoryFilter == value)
                return;

            _inlineFileCategoryFilter = value;
            AppStateStore.CurrentFileCategoryFilter = value;
        }
    }

    private ArtifactExplorerMode _artifactExplorerMode;

    private ArtifactExplorerMode ArtifactExplorerMode
    {
        get => _artifactExplorerMode;
        set
        {
            if (_artifactExplorerMode == value)
                return;
            SetArtifactExplorerMode(value);
        }
    }

    private SubscriptionToken ArtifactChangeSubscription { get; set; } = default!;

    private SortTypeEnum _currentSortType = SortTypeEnum.Name;
    private bool _isAscOrder = true;
    private bool _isArtifactExplorerLoading = false;
    private bool _isSearchArtifactExplorerLoading = false;
    private bool _isPinBoxLoading = true;
    private bool _isGoingBack;
    private bool _isInFileViewer;
    private Timer? _timer;
    private Task? _searchTask;

    [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
    [AutoInject] public IFileWatchService FileWatchService { get; set; } = default!;
    [AutoInject] public IFileLauncher FileLauncher { get; set; } = default!;

    [Parameter] public IPinService PinService { get; set; } = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public string? InitialPath { get; set; }

    private FsArtifact? ScrolledToArtifact { get; set; }

    protected override async Task OnInitAsync()
    {
        ArtifactChangeSubscription = EventAggregator
            .GetEvent<ArtifactChangeEvent>()
            .Subscribe(
                HandleChangedArtifacts,
                ThreadOption.BackgroundThread,
                keepSubscriberReferenceAlive: true);

        if (string.IsNullOrWhiteSpace(InitialPath))
        {
            var preArtifact = AppStateStore.CurrentMyDeviceArtifact;
            CurrentArtifact = preArtifact;
        }
        else
        {
            var filePath = Path.GetDirectoryName(InitialPath);
            var defaultArtifact = await FileService.GetArtifactAsync(filePath);
            CurrentArtifact = defaultArtifact;
        }

        InlineFileCategoryFilter = AppStateStore.CurrentFileCategoryFilter;

        _ = Task.Run(async () =>
        {
            await LoadPinsAsync();
            await InvokeAsync(StateHasChanged);
        });

        _ = Task.Run(async () =>
        {
            await LoadChildrenArtifactsAsync(CurrentArtifact);
            await InvokeAsync(StateHasChanged);
        });

        await base.OnInitAsync();
    }

    protected override async Task OnAfterFirstRenderAsync()
    {
        await ApplyIntentArtifactIfNeededAsync();
        await base.OnAfterFirstRenderAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if ((_isGoingBack || AppStateStore.ArtifactListScrollTopValue != null) && _timer == null)
        {
            _timer = new System.Timers.Timer(500);
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += async (s, e) => await ScrollTimerElapsed(s, e);
        }
    }

    private async Task ScrollTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _timer?.Stop();
        if (_isArtifactExplorerLoading)
        {
            _timer?.Start();
            return;
        }
        _timer?.Stop();
        if (_isGoingBack)
        {
            var result = await JSRuntime.InvokeAsync<bool>("getLastScrollPosition", _artifactExplorerRef?.ArtifactExplorerListRef);
            if (!result)
            {
                _timer?.Start();
                return;
            }

            _isGoingBack = false;
        }

        if (AppStateStore.ArtifactListScrollTopValue != null)
        {
            var result = await JSRuntime.InvokeAsync<bool>("setArtifactListTopScrollValue", _artifactExplorerRef?.ArtifactExplorerListRef, AppStateStore.ArtifactListScrollTopValue);
            if (!result)
            {
                _timer?.Start();
                return;
            }
            AppStateStore.ArtifactListScrollTopValue = null;
            await JSRuntime.InvokeVoidAsync("clearScrollTopValue");
        }

        DisposeTimer();
    }

    private void DisposeTimer()
    {
        if (_timer == null)
            return;

        _timer.Enabled = false;
        _timer.Stop();
        _timer.Dispose();
        _timer = null;
    }

    private async Task UpdateProgressAsync(
        string? text = null,
        string? subText = null,
        double? current = null,
        int? max = null)
    {
        if (text is not null)
            ProgressBarCurrentText = text;

        if (subText is not null)
            ProgressBarCurrentSubText = subText;

        if (current is not null)
            ProgressBarCurrentValue = current.Value;

        if (max is not null)
            ProgressBarMax = max.Value;

        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateProgressAsync(ProgressInfo progressInfo)
    {
        await UpdateProgressAsync(
            progressInfo.CurrentText,
            progressInfo.CurrentSubText,
            progressInfo.CurrentValue,
            progressInfo.MaxValue);
    }

    private async Task HandleCopyArtifactsAsync(List<FsArtifact> sourceArtifacts)
    {
        try
        {
            var destinationPath =
                await ShowDestinationSelectorModalAsync(Localizer.GetString(AppStrings.CopyHere), sourceArtifacts);

            if (string.IsNullOrWhiteSpace(destinationPath))
                return;

            await CloseFileViewer();

            ProgressBarCts = new CancellationTokenSource();

            if (destinationPath != CurrentArtifact?.FullPath)
            {
                await NavigateToAsync(destinationPath);

                await _progressModalRef!.ShowAsync(ProgressMode.Progressive,
                    Localizer.GetString(AppStrings.CopyFiles),
                    true);

                await Task.Run(async () =>
                {
                    bool? shouldOverwrite = null;

                    var notCopiedList = await FileService.CopyArtifactsAsync(
                        sourceArtifacts,
                        destinationPath,
                        onShouldOverwrite: async (artifact) =>
                        {
                            if (shouldOverwrite is null)
                            {
                                var result = await _confirmationReplaceOrSkipModalRef!.ShowAsync(artifact);

                                shouldOverwrite = result.ResultType ==
                                                  ConfirmationReplaceOrSkipModalResultType.Replace;
                            }

                            return shouldOverwrite.Value;
                        },
                        onProgress: UpdateProgressAsync,
                        cancellationToken: ProgressBarCts.Token);

                    if (notCopiedList.Any())
                    {
                        var knownException = notCopiedList.Select(a => a.exception)
                                                          .OfType<KnownException>()
                                                          .FirstOrDefault();

                        throw new DomainLogicException(knownException?.Message ??
                                                       AppStrings.TheCopyOpreationFailedMessage);
                    }
                });
            }
            else
            {
                await UpdateProgressAsync(
                    text: string.Empty,
                    subText: string.Empty,
                    current: 0,
                    max: 0);

                await _progressModalRef!.ShowAsync(
                    progressMode: ProgressMode.Progressive,
                    title: Localizer.GetString(AppStrings.CopyFiles),
                    isCancelable: true);

                await Task.Run(async () =>
                {
                    foreach (var sourceArtifact in sourceArtifacts)
                    {
                        ProgressBarCurrentValue += 0.5;
                        var roundedProgressCount = Math.Round(ProgressBarCurrentValue, MidpointRounding.AwayFromZero);

                        await UpdateProgressAsync(
                        text: sourceArtifact.Name,
                        subText: $"{roundedProgressCount} of {sourceArtifacts.Count}",
                        current: ProgressBarCurrentValue,
                        max: sourceArtifacts.Count);

                        switch (sourceArtifact.ArtifactType)
                        {
                            case FsArtifactType.File:
                                {

                                    if (sourceArtifact.ParentFullPath != null)
                                    {
                                        await CopyFileWithCopyPostfixAsync(sourceArtifact);
                                    }

                                    break;
                                }
                            case FsArtifactType.Folder:
                                {
                                    await CopyFolderWithCopyPostfixAsync(sourceArtifact);
                                    break;
                                }
                            case FsArtifactType.Drive:
                            default:
                                // ToDo : copy drive not supported, show proper message
                                break;
                        }

                        ProgressBarCurrentValue += 0.5;
                        roundedProgressCount = Math.Round(ProgressBarCurrentValue, MidpointRounding.AwayFromZero);

                        await UpdateProgressAsync(
                        text: sourceArtifact.Name,
                        subText: $"{roundedProgressCount} of {sourceArtifacts.Count}",
                        current: ProgressBarCurrentValue,
                        max: sourceArtifacts.Count);
                    }
                });
            }

            FxToast.Show(title: AppStrings.TheCopyOpreationSuccessedTiltle,
                             message: AppStrings.TheCopyOpreationSuccessedMessage,
                             toastType: FxToastType.Success);
        }
        catch (IOException ex)
        {
            ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
            ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            await _progressModalRef!.CloseAsync();
            ArtifactExplorerMode = ArtifactExplorerMode.Normal;
        }
    }

    private async Task CopyFolderWithCopyPostfixAsync(FsArtifact sourceArtifact)
    {
        try
        {
            var oldArtifactPath = sourceArtifact.FullPath;

            var oldArtifactParentPath = sourceArtifact.ParentFullPath;

            var oldArtifactName = sourceArtifact.Name;

            var counter = 0;
            string fullPathWithCopy;
            while (true)
            {
                fullPathWithCopy = $"{oldArtifactPath}{AppStrings.CopyPostfix}"
                                   + (counter > 1 ? $" {counter}" : string.Empty);

                var exists = (await FileService.CheckPathExistsAsync(new List<string?> { fullPathWithCopy }))?.First().IsExist ?? false;

                if (!exists)
                    break;

                counter++;
            }

            var newArtifactPath = fullPathWithCopy;

            var newArtifactName = Path.GetFileName(newArtifactPath);
            await FileService.CreateFolderAsync(oldArtifactParentPath, newArtifactName);

            var oldArtifactChildren =
                await FileService.GetArtifactsAsync(oldArtifactPath).ToListAsync();
            await FileService.CopyArtifactsAsync(oldArtifactChildren, newArtifactPath);
        }
        catch (IOException ex)
        {
            ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
            ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task<string> CopyFileWithCopyPostfixAsync(FsArtifact sourceArtifact)
    {
        var nameWithOutExtenstion = Path.GetFileNameWithoutExtension(sourceArtifact.FullPath);

        var pathWithOutExtenstion =
            Path.Combine(sourceArtifact.ParentFullPath, nameWithOutExtenstion);

        var oldArtifactPath = sourceArtifact.FullPath;

        var copyText = AppStrings.CopyPostfix;

        while (true)
        {
            var counter = 1;

            var fullPathWithCopy = pathWithOutExtenstion + copyText;

            fullPathWithCopy =
                Path.ChangeExtension(fullPathWithCopy, sourceArtifact.FileExtension);

            var exists = (await FileService.CheckPathExistsAsync(new List<string?> { fullPathWithCopy }))?.First().IsExist ?? false;
            if (!exists)
                break;

            counter++;
            copyText += $" ({counter})";
        }

        var newArtifactPath =
            Path.ChangeExtension(pathWithOutExtenstion + copyText,
                sourceArtifact.FileExtension);

        await FileService.CopyFileAsync(sourceArtifact, newArtifactPath);

        return newArtifactPath;
    }

    private async Task HandleMoveArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            var destinationPath =
                await ShowDestinationSelectorModalAsync(Localizer.GetString(AppStrings.MoveHere), artifacts);
            if (string.IsNullOrWhiteSpace(destinationPath))
                return;

            await CloseFileViewer();


            ProgressBarCts = new CancellationTokenSource();

            await NavigateToAsync(destinationPath);

            await _progressModalRef!.ShowAsync(ProgressMode.Progressive,
                Localizer.GetString(AppStrings.MovingFiles), true);

            await Task.Run(async () =>
            {
                bool? shouldOverwrite = null;
                var notMovedList = await FileService.MoveArtifactsAsync(artifacts,
                destinationPath,
                onShouldOverwrite: async (artifact) =>
                {
                    if (shouldOverwrite is null)
                    {
                        var result = await _confirmationReplaceOrSkipModalRef!.ShowAsync(artifact);

                        shouldOverwrite = result.ResultType ==
                                          ConfirmationReplaceOrSkipModalResultType.Replace;
                    }

                    return shouldOverwrite.Value;
                },
                onProgress: async (progressInfo) =>
                {
                    await UpdateProgressAsync(progressInfo);
                },
                cancellationToken: ProgressBarCts.Token);

                if (notMovedList.Any())
                {
                    var knownException = notMovedList.Select(a => a.exception)
                                                                  .OfType<KnownException>()
                                                                  .FirstOrDefault();

                    throw new DomainLogicException(knownException?.Message ??
                                                   AppStrings.TheMoveOpreationFailedMessage);
                }

            });

            FxToast.Show(title: AppStrings.TheMoveOpreationSuccessedTiltle,
                         message: AppStrings.TheMoveOpreationSuccessedMessage,
                         toastType: FxToastType.Success);
        }
        catch (IOException ex)
        {
            ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
            ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            await _progressModalRef!.CloseAsync();
            ArtifactExplorerMode = ArtifactExplorerMode.Normal;
        }
    }

    private async Task HandleRenameArtifactAsync(FsArtifact? artifact)
    {
        var oldPath = artifact?.FullPath;
        var result = await GetInputModalResult(artifact);
        if (result?.ResultType == InputModalResultType.Cancel)
        {
            return;
        }

        var newName = result?.Result;

        switch (artifact?.ArtifactType)
        {
            case FsArtifactType.Folder:
                await RenameFolderAsync(artifact, newName);
                break;
            case FsArtifactType.File:
                await RenameFileAsync(artifact, newName);
                break;
            case FsArtifactType.Drive:
                {
                    var title = Localizer.GetString(AppStrings.ToastErrorTitle);
                    var message = Localizer.GetString(AppStrings.RootfolderRenameException);
                    FxToast.Show(title, message, FxToastType.Error);
                    break;
                }
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        var newPath = artifact?.FullPath;
        if (newPath != null && oldPath != null)
            FileWatchService.UpdateFileWatchCatch(newPath, oldPath);
    }

    private async Task HandlePinArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            _isPinBoxLoading = true;
            await PinService.SetArtifactsPinAsync(artifacts);
            await UpdatePinedArtifactsAsync(artifacts, true);
            if (_isInSearchMode)
            {
                CancelSelectionMode();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            _isPinBoxLoading = false;
        }
    }

    private async Task HandleUnPinArtifactsAsync(List<FsArtifact> artifacts)
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
            ExceptionHandler.Handle(exception);
            _isPinBoxLoading = false;
        }
    }

    private async Task HandleDeleteArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            var result = new ConfirmationModalResult();

            if (artifacts.Count == 1)
            {
                var singleArtifact = artifacts.SingleOrDefault();
                if (singleArtifact?.Name != null)
                {
                    result = await _confirmationModalRef!.ShowAsync(
                        Localizer.GetString(AppStrings.DeleteItem, singleArtifact.Name),
                        Localizer.GetString(AppStrings.DeleteItemDescription));
                }
            }
            else
            {
                result = await _confirmationModalRef!.ShowAsync(
                    Localizer.GetString(AppStrings.DeleteItems, artifacts.Count),
                    Localizer.GetString(AppStrings.DeleteItemsDescription));
            }

            if (result.ResultType == ConfirmationModalResultType.Confirm)
            {
                ProgressBarCts = new CancellationTokenSource();

                await _progressModalRef!.ShowAsync(ProgressMode.Progressive,
                    Localizer.GetString(AppStrings.DeletingFiles), true);

                await Task.Run(async () =>
                {
                    await FileService.DeleteArtifactsAsync(artifacts,
                                                           onProgress: UpdateProgressAsync,
                                                           cancellationToken: ProgressBarCts.Token);
                });

                await _progressModalRef.CloseAsync();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            await _progressModalRef!.CloseAsync();
            await CloseFileViewer();
            ArtifactExplorerMode = ArtifactExplorerMode.Normal;
        }
    }

    private async Task HandleShowDetailsArtifact(List<FsArtifact> artifacts, bool shouldSkipOverflowAfterClose = false)
    {
        var isMultiple = artifacts.Count > 1;
        var isDrive = false;

        if (isMultiple is false)
        {
            isDrive = artifacts.SingleOrDefault()?.ArtifactType == FsArtifactType.Drive;
        }

        if (_artifactDetailModalRef is null)
            return;

        var result = await _artifactDetailModalRef.ShowAsync(artifacts, isMultiple, (isDrive || (IsInRoot(CurrentArtifact) && !_isInSearchMode)));

        switch (result.ResultType)
        {
            case ArtifactDetailModalResultType.Download:
                //TODO: Implement download logic here
                //await HandleDownloadArtifacts(artifact);
                break;
            case ArtifactDetailModalResultType.Move:
                await HandleMoveArtifactsAsync(artifacts);
                break;
            case ArtifactDetailModalResultType.Pin:
                await HandlePinArtifactsAsync(artifacts);
                break;
            case ArtifactDetailModalResultType.Unpin:
                await HandleUnPinArtifactsAsync(artifacts);
                break;
            case ArtifactDetailModalResultType.More:
                if (artifacts.Count > 1)
                {
                    await HandleSelectedArtifactsOptions(artifacts);
                }
                else
                {
                    await HandleOptionsArtifact(artifacts[0]);
                }

                break;
            case ArtifactDetailModalResultType.Upload:
                //TODO: Implement upload logic here
                break;
            case ArtifactDetailModalResultType.Close:
                if (shouldSkipOverflowAfterClose)
                {
                    return;
                }
                if (artifacts.Count > 1)
                {
                    await HandleSelectedArtifactsOptions(artifacts);
                }
                else
                {
                    await HandleOptionsArtifact(artifacts[0]);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task HandleCreateFolder(string path)
    {
        if (_inputModalRef is null) return;

        var createFolder = Localizer.GetString(AppStrings.CreateFolder);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.NewFolderPlaceholder);

        var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, string.Empty, newFolderPlaceholder);

        try
        {
            if (result.ResultType == InputModalResultType.Confirm)
            {
                await FileService.CreateFolderAsync(path,
                    result.Result);
            } //ToDo: Make CreateFolderAsync nullable         
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task HandleShareFiles(List<FsArtifact> artifacts)
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

    private async Task HandleExtractArtifactAsync(FsArtifact zipArtifact, List<FsArtifact>? innerArtifacts = null,
        string? destinationDirectory = null)
    {
        var extractResult = new ExtractorBottomSheetResult();
        if (_inputModalRef is null)
        {
            return;
        }

        var folderName = Path.GetFileNameWithoutExtension(zipArtifact.Name);
        var createFolder = Localizer.GetString(AppStrings.FolderName);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
        var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

        try
        {
            var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, folderName, newFolderPlaceholder,
                extractBtnTitle);

            if (result.ResultType == InputModalResultType.Cancel)
            {
                return;
            }

            var destinationFolderName = string.IsNullOrWhiteSpace(result?.Result) == false ? result.Result : folderName;

            destinationDirectory ??= zipArtifact.ParentFullPath;

            if (destinationDirectory != null)
            {
                if (_extractorModalRef == null)
                {
                    return;
                }

                extractResult = await _extractorModalRef.ShowAsync(zipArtifact.FullPath, destinationDirectory,
                    destinationFolderName, innerArtifacts);
            }

            if (destinationDirectory != null && extractResult.ExtractorResult == ExtractorBottomSheetResultType.Success)
            {
                var destinationPath = Path.Combine(destinationDirectory, destinationFolderName);
                await NavigateToAsync(destinationPath);
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task HandleOpenWithAppAsync(FsArtifact? artifact)
    {
        if (artifact?.FullPath == null)
            return;

        AppStateStore.IntentFileUrl = artifact.FullPath;
        var isOpen = await FileLauncher.OpenWithAsync(artifact.FullPath);
        if (isOpen)
        {
            await SecureStorage.Default.SetAsync("intentFilePath", artifact.FullPath);
        }
    }

    private List<ShareFile> GetShareFiles(List<FsArtifact> artifacts)
    {
        var filesQuery =
            from artifact in artifacts
            where artifact.ArtifactType == FsArtifactType.File
            select new ShareFile(artifact.FullPath);

        var files = filesQuery.ToList();
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
        }
    }

    private async Task LoadChildrenArtifactsAsync(FsArtifact? artifact = null)
    {
        try
        {
            _isArtifactExplorerLoading = true;

            var childrenArtifacts = FileService.GetArtifactsAsync(artifact?.FullPath);
            if (artifact is null)
            {
                GoBackService.SetState(null, true, true);
            }
            else
            {
                GoBackService.SetState(HandleToolbarBackClickAsync, true, false);
            }

            var artifacts = new List<FsArtifact>();
            await foreach (var item in childrenArtifacts)
            {
                item.IsPinned = await PinService.IsPinnedAsync(item);
                artifacts.Add(item);
            }

            _allArtifacts = artifacts;
            RefreshDisplayedArtifacts();
        }
        catch (IOException ex)
        {
            ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
        }
        catch (UnauthorizedAccessException ex)
        {
            ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
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

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null;
    }

    private async Task HandleSelectArtifactAsync(FsArtifact artifact)
    {
        if (artifact.ArtifactType == FsArtifactType.File)
        {
            _fxSearchInputRef?.HandleClearInputText();
            var isOpened = _fileViewerRef != null && await _fileViewerRef.OpenArtifact(artifact);
            _isInFileViewer = isOpened;

            if (isOpened == false)
            {
#if BlazorHybrid
                try
                {
                    await FileLauncher.OpenFileAsync(artifact.FullPath);
                }
                catch (UnauthorizedAccessException)
                {
                    ExceptionHandler.Handle(
                        new DomainLogicException(
                            Localizer.GetString(nameof(AppStrings.ArtifactUnauthorizedAccessException))));
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Handle(exception);
                }
#endif
            }
        }
        else
        {
            await OpenFolderAsync(artifact);
        }
    }

    private async Task OpenFolderAsync(FsArtifact artifact)
    {
        try
        {
            if (_isInSearchMode)
            {
                await CancelSearchAsync();
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("saveScrollPosition", _artifactExplorerRef?.ArtifactExplorerListRef);
                _isGoingBack = false;
            }

            await JSRuntime.InvokeVoidAsync("OnScrollEvent", _artifactExplorerRef?.ArtifactExplorerListRef);

            CurrentArtifact = artifact;
            _displayedArtifacts = new List<FsArtifact>();

            if (!string.IsNullOrWhiteSpace(_inlineSearchText))
            {
                _fxSearchInputRef?.HandleClearInputText();
                _inlineSearchText = string.Empty;
            }

            _ = Task.Run(async () =>
            {
                await LoadChildrenArtifactsAsync(CurrentArtifact);
                await InvokeAsync(StateHasChanged);
            });
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
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
            var isInMainFolder = CurrentArtifact?.FullPath == artifact?.FullPath;

            result = await _artifactOverflowModalRef!.ShowAsync
            (false,
                pinOptionResult,
                isDrive,
                artifact?.FileCategory,
                artifact?.ArtifactType,
                _isInSearchMode,
                _isInFileViewer,
                isInMainFolder);
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
                    ExceptionHandler.Handle(exception);
                }
                finally
                {
                    _isArtifactExplorerLoading = false;
                }

                break;
            case ArtifactOverflowResultType.OpenFileWithApp:
                await HandleOpenWithAppAsync(artifact);
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
            case ArtifactOverflowResultType.ShowInLocation:
                await NavigateArtifactForShowInFolder(artifact);
                break;
            case ArtifactOverflowResultType.Extract:
                await HandleExtractArtifactAsync(artifact);
                break;
            case ArtifactOverflowResultType.Cancel:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ToggleSelectedAll()
    {
        if (ArtifactExplorerMode != ArtifactExplorerMode.Normal)
            return;

        ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
        _selectedArtifacts = new List<FsArtifact>();
        foreach (var artifact in _displayedArtifacts)
        {
            artifact.IsSelected = true;
            _selectedArtifacts.Add(artifact);
        }
    }

    public void ChangeViewMode()
    {
        var viewMode = AppStateStore.ViewMode == ViewModeEnum.List ? ViewModeEnum.Grid : ViewModeEnum.List;
        AppStateStore.ViewMode = viewMode;
        StateHasChanged();
    }

    public void CancelSelectionMode()
    {
        foreach (var artifact in _selectedArtifacts)
        {
            artifact.IsSelected = false;
        }

        _selectedArtifacts.Clear();
        ArtifactExplorerMode = ArtifactExplorerMode.Normal;
    }

    private async Task HandleSelectedArtifactsOptions(List<FsArtifact> artifacts)
    {
        var selectedArtifactsCount = artifacts.Count;
        var isMultiple = selectedArtifactsCount > 1;

        if (selectedArtifactsCount <= 0) return;

        ArtifactOverflowResult? result = null;
        if (_artifactOverflowModalRef is not null)
        {
            ArtifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
            var pinOptionResult = GetPinOptionResult(artifacts);

            var firstArtifact = artifacts.FirstOrDefault();
            FileCategoryType? fileCategoryType = artifacts.All(x => x.FileCategory == firstArtifact?.FileCategory)
                ? firstArtifact?.FileCategory
                : null;
            FsArtifactType? fsArtifactType = artifacts.All(a => a.ArtifactType == firstArtifact?.ArtifactType)
                ? firstArtifact?.ArtifactType
                : null;

            result = await _artifactOverflowModalRef.ShowAsync
                (isMultiple,
                pinOptionResult,
                (IsInRoot(CurrentArtifact) && !_isInSearchMode),
                fileCategoryType,
                fsArtifactType,
                _isInFileViewer);
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
            case ArtifactOverflowResultType.OpenFileWithApp when (!isMultiple):
                var artifact = artifacts.SingleOrDefault();
                await HandleOpenWithAppAsync(artifact);
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
            case ArtifactOverflowResultType.Extract:
                await HandleExtractArtifactAsync(artifacts.First());
                break;
            case ArtifactOverflowResultType.Cancel:
                break;
            case ArtifactOverflowResultType.ShowInLocation:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetArtifactExplorerMode(ArtifactExplorerMode mode)
    {
        _artifactExplorerMode = mode;
        RefreshDeviceBackButtonBehavior();

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

        if (artifacts.All(a => a.IsPinned == false))
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
        var artifactType = "";

        switch (artifact?.ArtifactType)
        {
            case FsArtifactType.File:
                artifactType = Localizer.GetString(AppStrings.FileRenamePlaceholder);
                break;
            case FsArtifactType.Folder:
                artifactType = Localizer.GetString(AppStrings.FolderRenamePlaceholder);
                break;
            case null:
            case FsArtifactType.Drive:
            default:
                return null;
        }

        var name = Path.GetFileNameWithoutExtension(artifact.Name);

        InputModalResult? result = null;
        if (_inputModalRef is null)
            return result;

        result = await _inputModalRef.ShowAsync(Localizer.GetString(AppStrings.ChangeName),
            Localizer.GetString(AppStrings.Rename).ToString().ToUpper(), name, artifactType);

        return result;
    }

    private async Task<string?> ShowDestinationSelectorModalAsync(string buttonText, List<FsArtifact> artifacts)
    {
        if (_artifactSelectionModalRef is null)
            return null;

        var initialArtifactPath = artifacts.FirstOrDefault()?.ParentFullPath;

        var initialArtifact = string.IsNullOrWhiteSpace(initialArtifactPath)
            ? null
            : await FileService.GetArtifactAsync(initialArtifactPath);

        var result = await _artifactSelectionModalRef.ShowAsync(initialArtifact, buttonText, artifacts);

        string? destinationPath = null;

        if (result.ResultType != ArtifactSelectionResultType.Ok)
            return destinationPath;

        var destinationFsArtifact = result.SelectedArtifacts.FirstOrDefault();
        destinationPath = destinationFsArtifact?.FullPath;

        return destinationPath;
    }

    private readonly SemaphoreSlim _semaphoreArtifactChanged = new(1);

    private async void HandleChangedArtifacts(ArtifactChangeEvent artifactChangeEvent)
    {
        try
        {
            await _semaphoreArtifactChanged.WaitAsync();

            if (artifactChangeEvent.FsArtifact == null) return;

            switch (artifactChangeEvent.ChangeType)
            {
                case FsArtifactChangesType.Add:
                    _ = UpdateAddedArtifactAsync(artifactChangeEvent.FsArtifact);
                    break;
                case FsArtifactChangesType.Delete:
                    _ = UpdateRemovedArtifactAsync(artifactChangeEvent.FsArtifact);
                    break;
                case FsArtifactChangesType.Rename when
                    artifactChangeEvent.Description != null:
                    _ = UpdateRenamedArtifactAsync(artifactChangeEvent.FsArtifact, artifactChangeEvent.Description);
                    break;
                case FsArtifactChangesType.Modify:
                    _ = UpdateModifiedArtifactAsync(artifactChangeEvent.FsArtifact);
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
        finally
        {
            _semaphoreArtifactChanged.Release();
        }
    }

    private async Task UpdateAddedArtifactAsync(FsArtifact artifact)
    {
        try
        {
            if (artifact.ParentFullPath != CurrentArtifact?.FullPath) return;

            _allArtifacts.Add(artifact);
            RefreshDisplayedArtifacts();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task UpdateRemovedArtifactAsync(FsArtifact artifact)
    {
        try
        {
            _pins.RemoveAll(a => a.FullPath == artifact.FullPath);

            if (artifact.FullPath == CurrentArtifact?.FullPath)
            {
                await HandleToolbarBackClickAsync();
            }
            else
            {
                _allArtifacts.RemoveAll(a => a.FullPath == artifact.FullPath);
                RefreshDisplayedArtifacts();
            }

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task UpdateModifiedArtifactAsync(FsArtifact artifact)
    {
        try
        {
            var modifiedArtifact = _allArtifacts.FirstOrDefault(a => a.FullPath == artifact.FullPath);
            if (modifiedArtifact == null) return;

            modifiedArtifact.Size = artifact.Size;
            modifiedArtifact.LastModifiedDateTime = artifact.LastModifiedDateTime;

            RefreshDisplayedArtifacts();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task UpdateRenamedArtifactAsync(FsArtifact artifact, string oldFullPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(oldFullPath)) return;

            if (CurrentArtifact?.FullPath == oldFullPath)
            {
                CurrentArtifact.FullPath = artifact.FullPath;
                CurrentArtifact.Name = artifact.Name;
                await OpenFolderAsync(CurrentArtifact);
            }
            else
            {
                var artifactRenamed = _allArtifacts.FirstOrDefault(a => a.FullPath == oldFullPath);
                if (artifactRenamed != null)
                {
                    artifactRenamed.FullPath = artifact.FullPath;
                    artifactRenamed.Name = artifact.Name;
                    artifactRenamed.FileExtension = artifact.FileExtension;
                    RefreshDisplayedArtifacts();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task UpdatePinedArtifactsAsync(IEnumerable<FsArtifact> artifacts, bool isPinned)
    {
        await LoadPinsAsync();
        var artifactPath = artifacts.Select(a => a.FullPath).ToArray();
        if (CurrentArtifact != null && artifactPath.Any(p => p == CurrentArtifact.FullPath))
        {
            CurrentArtifact.IsPinned = isPinned;
        }
        else
        {
            var pinnedArtifacts = _allArtifacts.Where(artifact => artifactPath.Contains(artifact.FullPath));
            foreach (var artifact in pinnedArtifacts)
            {
                artifact.IsPinned = isPinned;
            }

            RefreshDisplayedArtifacts();
        }
    }

    private async Task HandleClearInLineSearchAsync()
    {
        _fxSearchInputRef?.HandleCancel();
        _inlineSearchText = string.Empty;
        await LoadChildrenArtifactsAsync(CurrentArtifact);
    }

    private async Task HandleCancelInLineSearchAsync()
    {
        ArtifactExplorerMode = ArtifactExplorerMode.Normal;
        _inlineSearchText = string.Empty;
        await LoadChildrenArtifactsAsync(CurrentArtifact);
    }

    private void HandleSearchFocused()
    {
        if (_isInSearchMode is false)
        {
            _isInSearchMode = true;
            _searchResultArtifacts.Clear();
        }

        _isSearchInputFocused = true;
        RefreshDeviceBackButtonBehavior();
    }

    private async Task UnFocuseSearchInputAsync()
    {
        await JSRuntime.InvokeVoidAsync("SearchInputUnFocus");
    }



    private async Task HandleSearchTextChangedAsync(string text)
    {
        CancelSelectionMode();
        if (string.IsNullOrWhiteSpace(text) && _artifactsSearchFilterTypes.Any() is false && _artifactsSearchFilterDate == null)
        {
            _isFileCategoryFilterBoxOpen = true;
        }
        else
        {
            _isFileCategoryFilterBoxOpen = false;
        }

        _isSearchArtifactExplorerLoading = true;
        _searchText = text;
        var searchFilter = GetSearchFilter();

        _searchResultArtifacts.Clear();
        _searchCancellationTokenSource?.Cancel();

        if (searchFilter.IsEmpty())
        {
            _isSearchArtifactExplorerLoading = false;
            _searchTask = Task.CompletedTask;
            return;
        }

        if (IsDesktop is false)
        {
            await UnFocuseSearchInputAsync();
        }

        _searchCancellationTokenSource = new CancellationTokenSource();
        var token = _searchCancellationTokenSource.Token;

        _searchTask = await Task.Factory.StartNew(async () =>
        {
            var bufferedArtifacts = new List<FsArtifact>();
            var isSearchComplete = false;
            _ = Task.Run(async () =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    while (bufferedArtifacts.Count > 0 || isSearchComplete is false)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), token);

                        if (bufferedArtifacts.Count == 0)
                            continue;

                        _searchResultArtifacts.AddRange(bufferedArtifacts);
                        _searchResultArtifacts = _searchResultArtifacts.ToList();
                        bufferedArtifacts.Clear();

                        if (_searchResultArtifacts.Count > 0 && _isSearchArtifactExplorerLoading)
                        {
                            _isSearchArtifactExplorerLoading = false;
                        }

                        await InvokeAsync(StateHasChanged);

                        sw.Restart();
                    }
                }
                catch (Exception exception) when (exception is not TaskCanceledException)
                {
                    ExceptionHandler.Handle(exception);
                }
                finally
                {
                    _isSearchArtifactExplorerLoading = false;
                }
            }, token);

            try
            {
                await foreach (var item in FileService.GetSearchArtifactAsync(searchFilter, token)
                                       .WithCancellation(token))
                {
                    if (token.IsCancellationRequested)
                        return;

                    item.IsPinned = await PinService.IsPinnedAsync(item);
                    bufferedArtifacts.Add(item);

                    await Task.Yield();
                }
            }
            catch (IOException ex)
            {
                ExceptionHandler.Handle(new KnownIOException(ex.Message, ex));
            }
            catch (UnauthorizedAccessException ex)
            {
                ExceptionHandler.Handle(new UnauthorizedException(ex.Message, ex));
            }

            isSearchComplete = true;
        }, token);
    }

    private DeepSearchFilter GetSearchFilter()
    {
        return new DeepSearchFilter()
        {
            SearchText = !string.IsNullOrWhiteSpace(_searchText) ? _searchText : string.Empty,
            ArtifactCategorySearchTypes = _artifactsSearchFilterTypes ?? null,
            ArtifactDateSearchType = _artifactsSearchFilterDate ?? null
        };
    }

    private void HandleInLineSearch(string? text)
    {
        if (text == null)
            return;

        RefreshDeviceBackButtonBehavior();
        _inlineSearchText = text;
        RefreshDisplayedArtifacts();
    }

    private async Task HandleToolbarBackClickAsync()
    {
        _inlineSearchText = string.Empty;
        _fxSearchInputRef?.HandleClearInputText();

        switch (ArtifactExplorerMode)
        {
            case ArtifactExplorerMode.Normal:
                if (_isInSearchMode)
                {
                    await CancelSearchAsync();
                    return;
                }

                await UpdateCurrentArtifactForBackButton(CurrentArtifact);
                _ = Task.Run(async () =>
                {
                    await LoadChildrenArtifactsAsync(CurrentArtifact);
                    await InvokeAsync(StateHasChanged);
                });
                await JSRuntime.InvokeVoidAsync("OnScrollEvent", _artifactExplorerRef?.ArtifactExplorerListRef);
                _isGoingBack = true;
                break;

            case ArtifactExplorerMode.SelectArtifact:
                ArtifactExplorerMode = ArtifactExplorerMode.Normal;
                break;

            case ArtifactExplorerMode.SelectDestination:
                ArtifactExplorerMode = ArtifactExplorerMode.Normal;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task UpdateCurrentArtifactForBackButton(FsArtifact? fsArtifact)
    {
        if (string.IsNullOrWhiteSpace(fsArtifact?.ParentFullPath))
        {
            CurrentArtifact = null;
            return;
        }

        CurrentArtifact = await FileService.GetArtifactAsync(fsArtifact.ParentFullPath);
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
        return InlineFileCategoryFilter is null
            ? artifacts
            : artifacts.Where(fa =>
            {
                if (InlineFileCategoryFilter == FileCategoryType.Document)
                {
                    return fa.FileCategory is FileCategoryType.Document
                        or FileCategoryType.Pdf
                        or FileCategoryType.Zip
                        or FileCategoryType.Other;
                }

                return fa.FileCategory == InlineFileCategoryFilter;
            });
    }

    private IEnumerable<FsArtifact> ApplySort(IEnumerable<FsArtifact> artifacts)
    {
        return SortDisplayedArtifacts(artifacts);
    }

    private async Task HandleFilterClick()
    {
        if (_isArtifactExplorerLoading || _filteredArtifactModalRef is null)
            return;

        InlineFileCategoryFilter = await _filteredArtifactModalRef.ShowAsync();
        await JSRuntime.InvokeVoidAsync("OnScrollEvent", _artifactExplorerRef?.ArtifactExplorerListRef);
        _isArtifactExplorerLoading = true;
        await Task.Run(() => { RefreshDisplayedArtifacts(); });
        _isArtifactExplorerLoading = false;
    }

    // TODO: septate variable for sort display variable and sort variable use case
    private async Task HandleSortOrderClick()
    {
        if (_isArtifactExplorerLoading) return;

        _isAscOrder = !_isAscOrder;
        _isArtifactExplorerLoading = true;
        await Task.Delay(100);
        try
        {
            var sortedDisplayArtifact = SortDisplayedArtifacts(_displayedArtifacts);
            _displayedArtifacts = new List<FsArtifact>();
            _displayedArtifacts = sortedDisplayArtifact.ToList();

            // For smooth transition and time for the animation to complete
            await Task.Delay(100);
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
        IEnumerable<FsArtifact> sortedArtifactsQuery = _currentSortType switch
        {
            SortTypeEnum.LastModified when _isAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.LastModifiedDateTime),
            SortTypeEnum.LastModified => artifacts
                .OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.LastModifiedDateTime),
            SortTypeEnum.Size when _isAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Size),
            SortTypeEnum.Size => artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.Size),
            SortTypeEnum.Name when _isAscOrder => artifacts
                .OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Name),
            SortTypeEnum.Name => artifacts.OrderByDescending(artifact => artifact.ArtifactType == FsArtifactType.Folder)
                .ThenByDescending(artifact => artifact.Name),
            _ => artifacts.OrderBy(artifact => artifact.ArtifactType != FsArtifactType.Folder)
                .ThenBy(artifact => artifact.Name)
        };

        return sortedArtifactsQuery;
    }

    private async Task RenameFileAsync(FsArtifact artifact, string? newName)
    {
        try
        {
            if (newName != null)
            {
                await FileService.RenameFileAsync(artifact.FullPath, newName);
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private async Task RenameFolderAsync(FsArtifact artifact, string? newName)
    {
        try
        {
            if (newName != null)
            {
                await FileService.RenameFolderAsync(artifact.FullPath, newName);
                artifact.Name = newName;
                var path = Path.GetDirectoryName(artifact.FullPath);
                if (path is null)
                    return;

                var newPath = Path.Combine(path, artifact.Name);
                artifact.FullPath = newPath;
                artifact.LocalFullPath = newPath;
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler.Handle(exception);
        }
    }

    private static List<FsArtifact> GetShouldOverwriteArtifacts(
        List<FsArtifact> sourceArtifacts,
        List<FsArtifact> existArtifacts)
    {
        return sourceArtifacts.Where(artifact => existArtifacts.Any(p => p.FullPath.StartsWith(artifact.FullPath))).ToList();
    }

    private async Task NavigateToAsync(string? destinationPath)
    {
        if (_isInSearchMode)
        {
            await CancelSearchAsync();
        }

        if (!string.IsNullOrWhiteSpace(_inlineSearchText))
        {
            _fxSearchInputRef?.HandleClearInputText();
            await HandleCancelInLineSearchAsync();
        }

        CurrentArtifact = await FileService.GetArtifactAsync(destinationPath);
        _ = LoadChildrenArtifactsAsync(CurrentArtifact);
    }

    private void RefreshDeviceBackButtonBehavior()
    {
        switch (ArtifactExplorerMode)
        {
            case ArtifactExplorerMode.SelectArtifact:
                GoBackService.SetState(Task () =>
                {
                    CancelSelectionMode();
                    return Task.CompletedTask;
                }, true, false);
                break;

            case ArtifactExplorerMode.Normal when CurrentArtifact == null && _isInSearchMode is false:
                GoBackService.SetState(null, true, true);
                break;

            case ArtifactExplorerMode.Normal when _isInSearchMode:
                GoBackService.SetState(async Task () =>
                {
                    if (string.IsNullOrWhiteSpace(_searchText))
                    {
                        await HandleToolbarBackClickAsync();
                    }
                    else
                    {
                        ClearSearch();
                    }

                    await Task.CompletedTask;
                }, true, false);
                break;

            case ArtifactExplorerMode.Normal:
                GoBackService.SetState(async Task () =>
                {
                    if (string.IsNullOrWhiteSpace(_inlineSearchText))
                    {
                        await HandleToolbarBackClickAsync();
                    }
                    else
                    {
                        await HandleClearInLineSearchAsync();
                    }

                    await Task.CompletedTask;
                }, true, false);
                break;

            case ArtifactExplorerMode.SelectDestination:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(ArtifactExplorerMode), ArtifactExplorerMode, null);
        }
    }

    private void ChangeFileCategoryFilterMode()
    {
        _isFileCategoryFilterBoxOpen = !_isFileCategoryFilterBoxOpen;
    }

    private async Task ChangeArtifactsSearchFilterDate(ArtifactDateSearchType? date)
    {
        _artifactsSearchFilterDate = _artifactsSearchFilterDate == date ? null : date;
        await HandleSearchTextChangedAsync(_searchText);
    }

    private async Task ChangeArtifactsSearchFilterType(ArtifactCategorySearchType type)
    {
        var isTypeExist = _artifactsSearchFilterTypes.Contains(type);
        if (isTypeExist)
        {
            _artifactsSearchFilterTypes.Remove(type);
        }
        else
        {
            _artifactsSearchFilterTypes.Add(type);
        }
        await HandleSearchTextChangedAsync(_searchText);
    }

    private async Task CancelSearchAsync()
    {
        ClearSearch();
        _isInSearchMode = false;
        await UnFocuseSearchInputAsync();
        StateHasChanged();
    }

    private void ClearSearch()
    {
        CancelSelectionMode();
        _searchCancellationTokenSource?.Cancel();
        _searchResultArtifacts.Clear();
        _fxToolBarRef?.HandleCancelSearch();
        _artifactsSearchFilterTypes.Clear();
        _artifactsSearchFilterDate = null;
        _isFileCategoryFilterBoxOpen = true;
        _searchText = string.Empty;
        RefreshDeviceBackButtonBehavior();
        StateHasChanged();
    }

    private async Task NavigateArtifactForShowInFolder(FsArtifact artifact)
    {
        var destinationArtifact = await FileService.GetArtifactAsync(artifact.ParentFullPath);
        CurrentArtifact = destinationArtifact;
        await OpenFolderAsync(destinationArtifact);
        ScrolledToArtifact = artifact;
    }

    private async Task CloseFileViewer()
    {
        if (_fileViewerRef is not null && _fileViewerRef.IsModalOpen)
        {
            await _fileViewerRef.HandleBackAsync();
        }
    }

    private async Task ApplyIntentArtifactIfNeededAsync()
    {
        if (AppStateStore.IntentFileUrl is null || _fileViewerRef is null)
            return;

        var uri = new Uri(AppStateStore.IntentFileUrl);
        FsFileProviderType fsFileProviderType = FsFileProviderType.InternalMemory;
        if (uri.Authority == "com.android.externalstorage.documents")
        {
            fsFileProviderType = FsFileProviderType.ExternalMemory;
        }

        var artifact = new FsArtifact(AppStateStore.IntentFileUrl, Path.GetFileName(AppStateStore.IntentFileUrl), FsArtifactType.File, fsFileProviderType)
        {
            FileExtension = Path.GetExtension(AppStateStore.IntentFileUrl),
            ParentFullPath = Directory.GetParent(AppStateStore.IntentFileUrl)?.FullName
        };

        await Task.Run(async () =>
        {
            await LoadChildrenArtifactsAsync(CurrentArtifact);
            await InvokeAsync(StateHasChanged);
        });

        AppStateStore.IntentFileUrl = null;
        await _fileViewerRef.OpenArtifact(artifact);
    }

    private async Task HandleOnArtifactTouchStartAsync()
    {
        if (_isSearchInputFocused)
        {
            _isSearchInputFocused = false;
            await UnFocuseSearchInputAsync();
        }
        await JSRuntime.InvokeVoidAsync("InlineSearchInputUnFocus");
    }

    private void SetSelectedArtifact(List<FsArtifact> artifacts)
    {
        _selectedArtifacts = artifacts;
        _searchPinOptionResult = GetPinOptionResult(_selectedArtifacts);
    }

    private void ProgressBarOnCancel()
    {
        ProgressBarCts?.Cancel();
    }

    public void Dispose()
    {
        ProgressBarCts?.Dispose();
        _timer?.Dispose();
        _searchTask?.Dispose();
        _semaphoreArtifactChanged.Dispose();
        _searchCancellationTokenSource?.Dispose();
        ArtifactChangeSubscription.Dispose();
    }
}