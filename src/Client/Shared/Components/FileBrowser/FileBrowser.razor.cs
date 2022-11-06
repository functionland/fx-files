using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Services.Common;
using Functionland.FxFiles.Client.Shared.Utils;

using Prism.Events;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser
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

    // Search
    private DeepSearchFilter? SearchFilter { get; set; }
    private bool _isFileCategoryFilterBoxOpen = true;
    private bool _isInSearch;
    private bool isFirstTimeInSearch = true;
    private string _inlineSearchText = string.Empty;
    private string _searchText = string.Empty;
    private ArtifactDateSearchType? _artifactsSearchFilterDate;
    private ArtifactCategorySearchType? _artifactsSearchFilterType;

    private FsArtifact? _currentArtifactValue;
    private FsArtifact? _currentArtifact
    {
        get => _currentArtifactValue;
        set
        {
            if (_currentArtifactValue != value)
            {
                if (_currentArtifactValue is not null)
                {
                    FileWatchService.UnWatchArtifact(_currentArtifactValue);
                }
                _currentArtifactValue = value;
                if (_currentArtifactValue is not null)
                {
                    FileWatchService.WatchArtifact(_currentArtifactValue);
                }
            }

            ArtifactState.SetCurrentMyDeviceArtifact(_currentArtifact);
        }
    }

    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _displayedArtifacts = new();
    private List<FsArtifact> _selectedArtifacts = new();
    private FileCategoryType? _fileCategoryFilter;

    private Tuple<FsArtifact, List<FsArtifact>?, string?, string?>? _extractTuple;

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
    private FileViewerResultType FileViewerResult { get; set; }

    [AutoInject] public IAppStateStore ArtifactState { get; set; } = default!;
    [AutoInject] public IEventAggregator EventAggregator { get; set; } = default!;
    [AutoInject] public IFileWatchService FileWatchService { get; set; } = default!;
    [AutoInject] public IZipService ZipService { get; set; } = default!;
    public SubscriptionToken ArtifactChangeSubscription { get; set; } = default!;

    [Parameter] public IPinService PinService { get; set; } = default!;
    [Parameter] public IFileService FileService { get; set; } = default!;
    [Parameter] public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;
    [Parameter] public string? DefaultPath { get; set; }

    protected override async Task OnInitAsync()
    {
        ArtifactChangeSubscription = EventAggregator
                           .GetEvent<ArtifactChangeEvent>()
                           .Subscribe(
                               HandleChangedArtifacts,
                               ThreadOption.BackgroundThread, keepSubscriberReferenceAlive: true);

        Task PinTask = LoadPinsAsync();
        Task ArtifactListTask;

        if (string.IsNullOrWhiteSpace(DefaultPath))
        {
            var preArtifact = ArtifactState.CurrentMyDeviceArtifact;
            if (preArtifact is null)
            {
                ArtifactListTask = LoadChildrenArtifactsAsync();
            }
            else
            {
                _currentArtifact = preArtifact;
                ArtifactListTask = LoadChildrenArtifactsAsync(preArtifact);
            }
        }
        else
        {
            var filePath = Path.GetDirectoryName(DefaultPath);
            var defaultArtifact = await FileService.GetArtifactAsync(filePath);
            _currentArtifact = defaultArtifact;
            ArtifactListTask = LoadChildrenArtifactsAsync(defaultArtifact);
        }

        await base.OnInitAsync();

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isInSearch && isFirstTimeInSearch)
        {
            await JSRuntime.InvokeVoidAsync("SearchInputFocus");
            isFirstTimeInSearch = false;
        }
        if (_isGoingBack)
        {
            _isGoingBack = false;
            await JSRuntime.InvokeVoidAsync("getLastScrollPossition");
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    private void HandleProgressBar(string CurrentText)
    {
        ProgressBarCurrentValue++;
        ProgressBarCurrentSubText = $"{ProgressBarCurrentValue} of {ProgressBarMax}";
        ProgressBarCurrentText = CurrentText;
    }

    private void InitialProgressBar(int maxValue)
    {
        ProgressBarCurrentValue = 0;
        ProgressBarMax = maxValue;
        ProgressBarCurrentSubText = string.Empty;
        ProgressBarCurrentText = "Loading...";
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
                return;

            if (_progressModalRef is not null)
            {
                InitialProgressBar(artifacts.Count);
                await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.CopyFiles), true);
            }
            ProgressBarCts = new CancellationTokenSource();

            if (destinationPath == _currentArtifact?.FullPath)
            {
                var desArtifacts = await FileService.GetArtifactsAsync(destinationPath).ToListAsync();

                foreach (var item in artifacts)
                {
                    if (item.ArtifactType == FsArtifactType.File)
                    {
                        var nameWithOutExtention = Path.GetFileNameWithoutExtension(item.FullPath);
                        var pathWithOutExtention = Path.Combine(item.ParentFullPath, nameWithOutExtention);
                        var newArtifactPath = string.Empty;
                        var oldArtifactPath = item.FullPath;

                        var copyText = " - Copy";

                        while (true)
                        {
                            var counter = 1;
                            var fullPathWithCopy = pathWithOutExtention + copyText;
                            fullPathWithCopy = Path.ChangeExtension(fullPathWithCopy, item.FileExtension);

                            if (!desArtifacts.Any(d => d.FullPath == fullPathWithCopy)) break;

                            counter++;
                            copyText += $" ({counter})";
                        }

                        newArtifactPath = Path.ChangeExtension(pathWithOutExtention + copyText, item.FileExtension);

                        var fileStream = await FileService.GetFileContentAsync(oldArtifactPath);
                        await FileService.CreateFileAsync(newArtifactPath, fileStream);
                    }
                    else if (item.ArtifactType == FsArtifactType.Folder)
                    {
                        var newArtifactPath = string.Empty;
                        var oldArtifactPath = item.FullPath;
                        var oldArtifactParentPath = item.ParentFullPath;
                        var oldArtifactName = item.Name;

                        var copyText = " - Copy";

                        while (true)
                        {
                            var counter = 1;
                            var fullPathWithCopy = oldArtifactPath + copyText;

                            if (!desArtifacts.Any(d => d.FullPath == fullPathWithCopy)) break;

                            counter++;
                            copyText += $" ({counter})";
                        }

                        newArtifactPath = oldArtifactPath + copyText;
                        var newArtifactName = oldArtifactName + copyText;
                        await FileService.CreateFolderAsync(oldArtifactParentPath, newArtifactName);
                        var oldArtifactChilds = await FileService.GetArtifactsAsync(oldArtifactPath).ToListAsync();
                        await FileService.CopyArtifactsAsync(oldArtifactChilds, newArtifactPath, false);
                    }
                    else
                    {
                        // ToDo : copy drive not supported, show proper message
                    }

                    HandleProgressBar(item.Name);
                }
            }
            else
            {
                try
                {
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

                var overwriteArtifacts = GetShouldOverwriteArtifacts(artifacts, existArtifacts); //TODO: we must enhance this

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
            }

            var title = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedMessage);
            FxToast.Show(title, message, FxToastType.Success);

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
            await CloseFileViewer();
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

                await CloseFileViewer();
            }

            var overwriteArtifacts = GetShouldOverwriteArtifacts(artifacts, existArtifacts); //TODO: we must enhance this

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
            FxToast.Show(title, message, FxToastType.Success);

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

        string? newName = result?.Result;

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
            FxToast.Show(title, message, FxToastType.Error);
        }
    }

    public async Task HandlePinArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            _isPinBoxLoading = true;
            await PinService.SetArtifactsPinAsync(artifacts);
            await UpdatePinedArtifactsAsync(artifacts, true);
            if (_isInSearch)
            {
                CancelSelectionMode();
            }
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
                            await InvokeAsync(() => StateHasChanged());
                        }, cancellationToken: ProgressBarCts.Token);

                        await _progressModalRef.CloseAsync();
                    }
                }
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }

        finally
        {

            if (_progressModalRef is not null)
            {
                ProgressBarCts?.Cancel();
                await _progressModalRef.CloseAsync();
            }
            await CloseFileViewer();
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
                await FileService.CreateFolderAsync(path, result?.Result); //ToDo: Make CreateFolderAsync nullable         
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

    // TODO: change tuple item for real names.
    public async Task HandleExtractArtifactAsync(Tuple<FsArtifact, List<FsArtifact>?, string?, string?> extractTuple)
    {
        var artifact = extractTuple.Item1;
        var innerArtifacts = extractTuple.Item2;
        var destinationDirectory = extractTuple.Item3 ?? _currentArtifact?.FullPath;
        var artifactPassword = extractTuple.Item4;
        if (_inputModalRef is null)
        {
            FileViewerResult = FileViewerResultType.Cancel;
            return;
        }

        var folderName = Path.GetFileNameWithoutExtension(artifact.Name);
        var createFolder = Localizer.GetString(AppStrings.FolderName);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.ExtractFolderTargetNamePlaceHolder);
        var extractBtnTitle = Localizer.GetString(AppStrings.Extract);

        try
        {
            var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, folderName, newFolderPlaceholder, extractBtnTitle);
            //var parentPath = artifact?.ParentFullPath ?? Directory.GetParent(artifact!.FullPath)?.FullName;

            if (result?.ResultType == InputModalResultType.Cancel)
            {
                FileViewerResult = FileViewerResultType.Cancel;
                return;
            }

            var destinationFolderName = result?.Result ?? folderName;
            try
            {
                if (destinationDirectory != null)
                    await ExtractZipAsync(artifact.FullPath, destinationDirectory, destinationFolderName,
                        artifactPassword, innerArtifacts);
            }
            catch (InvalidPasswordException)
            {
                if (_passwordModalRef is null)
                {
                    FileViewerResult = FileViewerResultType.Cancel;
                    return;
                }

                var extractPasswordModalTitle = Localizer.GetString(AppStrings.ExtractPasswordModalTitle);
                var extractPasswordModalLabel = Localizer.GetString(AppStrings.Password);
                var passwordResult = await _passwordModalRef.ShowAsync(extractPasswordModalTitle, string.Empty, string.Empty, string.Empty, extractBtnTitle, extractPasswordModalLabel);
                if (passwordResult?.ResultType == InputModalResultType.Cancel)
                {
                    FileViewerResult = FileViewerResultType.Cancel;
                    return;
                }

                if (destinationDirectory != null)
                    await ExtractZipAsync(artifact.FullPath, destinationDirectory, destinationFolderName,
                        passwordResult?.Result, innerArtifacts);
            }

            if (destinationDirectory != null)
            {
                var destinationPath = Path.Combine(destinationDirectory, destinationFolderName);
                await NavigateToDestionation(destinationPath);
            }

            FileViewerResult = FileViewerResultType.Success;
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            ChangeDeviceBackFunctionality(_artifactExplorerMode);
        }

    }

    private async Task ExtractZipAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName, string? password = null, List<FsArtifact>? innerArtifacts = null)
    {
        if (_progressModalRef is null) return;

        try
        {
            await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ExtractingFolder), true);
            ProgressBarCts = new CancellationTokenSource();

            async Task OnProgress(ProgressInfo progressInfo)
            {
                ProgressBarCurrentText = progressInfo.CurrentText ?? string.Empty;
                ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? string.Empty;
                ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                ProgressBarMax = progressInfo.MaxValue ?? 1;
                await InvokeAsync(StateHasChanged);
            }

            var duplicateCount = await ZipService.ExtractZippedArtifactAsync(
                zipFilePath,
                destinationFolderPath,
                destinationFolderName,
                innerArtifacts,
                 false,
                 password,
                 OnProgress,
                 ProgressBarCts.Token);

            await _progressModalRef.CloseAsync();

            if (duplicateCount <= 0) return;

            if (_confirmationReplaceOrSkipModalRef == null)
            {
                FileViewerResult = FileViewerResultType.Cancel;
                return;
            }

            var existedArtifacts = await FileService.GetArtifactsAsync(destinationFolderPath).ToListAsync();
            List<FsArtifact> overwriteArtifacts = new();
            if (innerArtifacts != null)
            {
                overwriteArtifacts = GetShouldOverwriteArtifacts(innerArtifacts, existedArtifacts);
            }

            var replaceResult = await _confirmationReplaceOrSkipModalRef.ShowAsync(duplicateCount);

            if (replaceResult?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
            {

                await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ReplacingFiles), true);

                ProgressBarCts = new CancellationTokenSource();
                await ZipService.ExtractZippedArtifactAsync(
                    zipFilePath,
                    destinationFolderPath,
                    destinationFolderName,
                    overwriteArtifacts,
                     true,
                     password,
                     OnProgress,
                     ProgressBarCts.Token);
            }
        }
        finally
        {
            await _progressModalRef.CloseAsync();
            ChangeDeviceBackFunctionality(_artifactExplorerMode);
        }
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
        StateHasChanged();

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
            // call _displayArtifact
            _displayedArtifacts = new();
            RefreshDisplayedArtifacts();
        }
        catch (ArtifactUnauthorizedAccessException exception)
        {
            ExceptionHandler?.Handle(exception);
        }
        finally
        {
            //trick for update load artifact and refresh visualization
            await Task.Delay(100);
            _isArtifactExplorerLoading = false;


            // check functionality
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
            var isOpened = await _fileViewerRef?.OpenArtifact(artifact);

            if (isOpened == false)
            {
#if BlazorHybrid
                try
                {

                    if (DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.macOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
                    {
                        var uri = new Uri($"file://{artifact.FullPath}");
                        await Launcher.OpenAsync(uri);

                    }
                    else
                    {
                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(artifact?.FullPath)
                        });
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ExceptionHandler?.Handle(new DomainLogicException(Localizer.GetString(nameof(AppStrings.ArtifactUnauthorizedAccessException))));
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Handle(exception);
                }

                if (_isInSearch)
                {
                    CancelSearch(true);
                    _currentArtifact = artifact;
                    await LoadChildrenArtifactsAsync(_currentArtifact);
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
            if (_isInSearch)
            {
                CancelSearch(true);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("saveScrollPosition");
                _isGoingBack = false;
            }
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
            result = await _artifactOverflowModalRef!.ShowAsync(false, pinOptionResult, isVisibleShareWithApp, artifact?.FileCategory, isDrive);
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
            case ArtifactOverflowResultType.Extract:
                if (artifact != null)
                {
                    _extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?, string?>(artifact, null, null, null);
                }

                if (_extractTuple != null)
                {
                    await HandleExtractArtifactAsync(_extractTuple);
                }
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

        if (selectedArtifactsCount <= 0) return;

        ArtifactOverflowResult? result = null;
        if (_artifactOverflowModalRef is not null)
        {
            _artifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
            var pinOptionResult = GetPinOptionResult(artifacts);
            var isVisibleSahreWithApp = !artifacts.Any(a => a.ArtifactType != FsArtifactType.File);

            var firstArtifactType = artifacts.FirstOrDefault()?.FileCategory;
            FileCategoryType? fileCategoryType = artifacts.All(x => x.FileCategory == firstArtifactType) ? firstArtifactType : null;

            result = await _artifactOverflowModalRef!.ShowAsync(isMultiple, pinOptionResult, isVisibleSahreWithApp, fileCategoryType, IsInRoot(_currentArtifact));
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
            case ArtifactOverflowResultType.Extract:

                _extractTuple = new Tuple<FsArtifact, List<FsArtifact>?, string?, string?>(artifacts.First(), null, null, null);

                if (_extractTuple != null)
                {
                    await HandleExtractArtifactAsync(_extractTuple);
                }
                break;
            case ArtifactOverflowResultType.Cancel:
                _artifactExplorerMode = ArtifactExplorerMode.Normal;
                break;
        }

        _artifactExplorerMode = ArtifactExplorerMode.Normal;
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

    private readonly SemaphoreSlim _semaphoreArtifactChanged = new SemaphoreSlim(1);
    private async void HandleChangedArtifacts(ArtifactChangeEvent artifactChangeEvent)
    {
        try
        {
            await _semaphoreArtifactChanged.WaitAsync();

            if (artifactChangeEvent.FsArtifact == null) return;

            if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Add)
            {
                _ = UpdateAddedArtifactAsync(artifactChangeEvent.FsArtifact);
            }
            else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Delete)
            {
                _ = UpdateRemovedArtifactAsync(artifactChangeEvent.FsArtifact);
            }
            else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Rename && artifactChangeEvent.Description != null)
            {
                _ = UpdateRenamedArtifactAsync(artifactChangeEvent.FsArtifact, artifactChangeEvent.Description);
            }
            else if (artifactChangeEvent.ChangeType == FsArtifactChangesType.Modify)
            {
                _ = UpdateModefiedArtifactAsync(artifactChangeEvent.FsArtifact);
            }
        }
        catch (Exception exp)
        {
            ExceptionHandler.Handle(exp);
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
            if (artifact.ParentFullPath != _currentArtifact?.FullPath) return;

            _allArtifacts.Add(artifact);
            RefreshDisplayedArtifacts();
            await InvokeAsync(() => StateHasChanged());
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
        }
    }

    private async Task UpdateRemovedArtifactAsync(FsArtifact artifact)
    {
        try
        {
            if (artifact.FullPath == _currentArtifact?.FullPath)
            {
                await HandleToolbarBackClick();
                return;
            }
            _allArtifacts.RemoveAll(a => a.FullPath == artifact.FullPath);
            RefreshDisplayedArtifacts();
            await InvokeAsync(() => StateHasChanged());
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
        }
    }

    private async Task UpdateModefiedArtifactAsync(FsArtifact artifact)
    {
        try
        {
            var modefiedArtifact = _allArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
            if (modefiedArtifact == null) return;

            modefiedArtifact.Size = artifact.Size;
            modefiedArtifact.LastModifiedDateTime = artifact.LastModifiedDateTime;

            RefreshDisplayedArtifacts();
            await InvokeAsync(() => StateHasChanged());

        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
        }
    }

    private async Task UpdateRenamedArtifactAsync(FsArtifact artifact, string oldFullPath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(oldFullPath)) return;

            FsArtifact? artifactRenamed = null;

            if (_currentArtifact?.FullPath == oldFullPath)
            {
                _currentArtifact.FullPath = artifact.FullPath;
                _currentArtifact.Name = artifact.Name;
                await OpenFolderAsync(_currentArtifact);
            }
            else
            {
                artifactRenamed = _allArtifacts.Where(a => a.FullPath == oldFullPath).FirstOrDefault();
                if (artifactRenamed != null)
                {
                    artifactRenamed.FullPath = artifact.FullPath;
                    artifactRenamed.Name = artifact.Name;
                    artifactRenamed.FileExtension = artifact.FileExtension;
                    RefreshDisplayedArtifacts();
                    await InvokeAsync(() => StateHasChanged());
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionHandler.Handle(ex);
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

    private async Task HandleCancelInLineSearchAsync()
    {
        //_isLoading = true;
        _artifactExplorerMode = ArtifactExplorerMode.Normal;
        _inlineSearchText = string.Empty;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        //_isLoading = false;
    }

    private void HandleSearchFocused()
    {
        _isInSearch = true;
    }

    CancellationTokenSource? searchCancellationTokenSource;

    private async Task HandleSearchAsync(string text)
    {
        CancelSelectionMode();
        _isArtifactExplorerLoading = true;
        _searchText = text;
        if (!string.IsNullOrWhiteSpace(text))
        {
            ApplySearchFilter(text, _artifactsSearchFilterDate, _artifactsSearchFilterType);
        }
        else
        {
            CancelSearch();
        }
        _allArtifacts.Clear();
        _displayedArtifacts.Clear();

        RefreshDisplayedArtifacts();

        if (searchCancellationTokenSource is not null)
        {
            searchCancellationTokenSource.Cancel();
        }

        searchCancellationTokenSource = new CancellationTokenSource();
        var token = searchCancellationTokenSource.Token;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        await Task.Run(async () =>
        {
            var buffer = new List<FsArtifact>();
            try
            {
                await foreach (var item in FileService.GetSearchArtifactAsync(SearchFilter, token))
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
                            if (_displayedArtifacts.Count > 0 && _isArtifactExplorerLoading)
                            {
                                _isArtifactExplorerLoading = false;
                            }
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
            catch (Exception)
            {
                //ExceptionHandler.Handle(ex);
            }
            finally
            {
                _isArtifactExplorerLoading = false;
            }

        });
    }

    private void ApplySearchFilter(string searchText, ArtifactDateSearchType? date = null, ArtifactCategorySearchType? type = null)
    {
        if (SearchFilter == null)
        {
            SearchFilter = new();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                SearchFilter.SearchText = searchText;
            }
            else
            {
                SearchFilter = null;
                return;
            }
            SearchFilter.ArtifactDateSearchType = date ?? null;

            SearchFilter.ArtifactCategorySearchType = type ?? null;

            return;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                SearchFilter.SearchText = searchText;
            }
            else
            {
                SearchFilter = null;
                return;
            }
            SearchFilter.ArtifactDateSearchType = date ?? null;

            SearchFilter.ArtifactCategorySearchType = type ?? null;

            return;
        }
    }

    private void HandleInLineSearch(string text)
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

        switch (_artifactExplorerMode)
        {
            case ArtifactExplorerMode.Normal:
                if (_isInSearch)
                {
                    CancelSearch(true);
                    await LoadChildrenArtifactsAsync(_currentArtifact);
                    return;
                }
                _fxSearchInputRef?.HandleClearInputText();
                await UpdateCurrentArtifactForBackButton(_currentArtifact);
                await LoadChildrenArtifactsAsync(_currentArtifact);
                await JSRuntime.InvokeVoidAsync("OnScrollEvent");
                _isGoingBack = true;
                break;

            case ArtifactExplorerMode.SelectArtifact:
                _artifactExplorerMode = ArtifactExplorerMode.Normal;
                break;

            case ArtifactExplorerMode.SelectDestionation:
                _artifactExplorerMode = ArtifactExplorerMode.Normal;
                break;

            default:
                break;
        }
        await InvokeAsync(() => StateHasChanged());
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
            _displayedArtifacts = new();
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

    private async Task RenameFileAsync(FsArtifact artifact, string? newName)
    {
        try
        {
            await FileService.RenameFileAsync(artifact.FullPath, newName);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private async Task RenameFolderAsync(FsArtifact artifact, string? newName)
    {
        try
        {
            await FileService.RenameFolderAsync(artifact.FullPath, newName);
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private static List<FsArtifact> GetShouldOverwriteArtifacts(List<FsArtifact> artifacts, List<FsArtifact> existArtifacts)
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
        if (_isInSearch)
        {
            CancelSearch(true);
        }
        _currentArtifact = await FileService.GetArtifactAsync(destinationPath);
        _ = LoadChildrenArtifactsAsync(_currentArtifact);
        _ = LoadPinsAsync();
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
            if (_currentArtifact == null && _isInSearch is false)
            {
                GoBackService.OnInit(null, true, true);
            }
            else
            {
                if (_isInSearch)
                {
                    GoBackService.OnInit((Task () =>
                    {
                        CancelSearch();
                        return Task.CompletedTask;
                    }), true, false);
                }
                else
                {
                    GoBackService.OnInit((async Task () =>
                    {
                        await HandleToolbarBackClick();
                        await Task.CompletedTask;
                    }), true, false);
                }
            }
        }
    }

    private void ChangeFileCategoryFilterMode()
    {
        _isFileCategoryFilterBoxOpen = !_isFileCategoryFilterBoxOpen;
    }

    private async Task ChangeArtifactsSearchFilterDate(ArtifactDateSearchType? date)
    {
        CancelSearch();
        _artifactsSearchFilterDate = _artifactsSearchFilterDate == date ? null : date;
        await HandleSearchAsync(_searchText);
    }

    private async Task ChangeArtifactsSearchFilterType(ArtifactCategorySearchType? type)
    {
        CancelSearch();
        _artifactsSearchFilterType = _artifactsSearchFilterType == type ? null : type;
        await HandleSearchAsync(_searchText);
    }

    private void CancelSearch(bool shouldExist = false)
    {
        searchCancellationTokenSource?.Cancel();
        SearchFilter = null;
        _fxSearchInputRef?.HandleClearInputText();
        _displayedArtifacts.Clear();
        CancelSelectionMode();
        _isInSearch = shouldExist is false ? true : false;
        if (shouldExist)
        {
            _artifactsSearchFilterType = null;
            _artifactsSearchFilterDate = null;
            isFirstTimeInSearch = true;
        }
    }

    private async Task NavigateArtifactForShowInFolder(FsArtifact artifact)
    {
        if (artifact.ArtifactType == FsArtifactType.File)
        {
            var destinationArtifact = await FileService.GetArtifactAsync(artifact.ParentFullPath);
            _currentArtifact = destinationArtifact;
            await HandleSelectArtifactAsync(destinationArtifact);
        }
        else
        {
            _currentArtifact = artifact;
            await HandleSelectArtifactAsync(artifact);
        }
    }

    private async Task CloseFileViewer()
    {
        if (_fileViewerRef is not null && _fileViewerRef.IsModalOpen)
        {
            await _fileViewerRef.HandleBackAsync();
        }
    }
}