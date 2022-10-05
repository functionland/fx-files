﻿using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Models;

using Microsoft.VisualBasic;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;
    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _filteredArtifacts = new();

    private InputModal? _inputModalRef;
    private ToastModal? _toastModalRef;
    private ConfirmationModal? _confirmationModalRef;
    private FilterArtifactModal? _filteredArtifactModalRef;
    private SortArtifactModal? _sortedArtifactModalRef;
    private ArtifactOverflowModal? _artifactOverflowModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private ConfirmationReplaceOrSkipModal? _confirmationReplaceOrSkipModalRef;
    private ArtifactDetailModal? _artifactDetailModalRef;
    private FxSearchInput? _fxSearchInputRef;
    private FsArtifact[] _selectedArtifacts { get; set; } = Array.Empty<FsArtifact>();
    private ArtifactActionResult _artifactActionResult { get; set; } = new();

    private string? _searchText;
    private bool _isInSearchMode;
    private ViewModeEnum _viewMode = ViewModeEnum.list;
    private FileCategoryType? _fileCategoryFilter;
    private ArtifactExplorerMode _artifactExplorerMode;
    private SortTypeEnum _currentSortType = SortTypeEnum.Name;
    private bool _isAscOrder = true;
    private bool _isSelected;

    [Parameter] public IPinService PinService { get; set; } = default!;

    [Parameter] public IFileService FileService { get; set; } = default!;

    protected override async Task OnInitAsync()
    {
        await LoadPinsAsync();

        await LoadChildrenArtifactsAsync();

        await base.OnInitAsync();
    }

    public async Task HandleCopyArtifactsAsync(FsArtifact[] artifacts)
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
                await FileService.CopyArtifactsAsync(artifacts, destinationPath, false);
            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }

            var overwriteArtifacts = GetShouldOverwriteArtiacts(artifacts, existArtifacts); //TODO: we must enhance this

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef != null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts.Count);
                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.CopyArtifactsAsync(overwriteArtifacts.ToArray(), destinationPath, true);
                    }
                }
            }

            var title = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Success);

            await NavigateToDestionation(destinationPath);
        }
        catch (DomainLogicException ex) when (ex is SameDestinationFolderException or SameDestinationFileException)
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            _toastModalRef!.Show(Title, ex.Message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
    }

    public async Task HandleMoveArtifactsAsync(FsArtifact[] artifacts)
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
                await FileService.MoveArtifactsAsync(artifacts, destinationPath, false);
            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }

            catch
            {

            }

            var overwriteArtifacts = GetShouldOverwriteArtiacts(artifacts, existArtifacts); //TODO: we must enhance this

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef is not null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts.Count);
                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.MoveArtifactsAsync(overwriteArtifacts.ToArray(), destinationPath, true);
                    }
                }
            }

            ArtifactExplorerModeChange(ArtifactExplorerMode.Normal);

            var title = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Success);

            await NavigateToDestionation(destinationPath);
        }
        catch (DomainLogicException ex) when (ex is SameDestinationFolderException or SameDestinationFileException)
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            _toastModalRef!.Show(Title, ex.Message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
    }

    public async Task<string?> HandleSelectDestinationAsync(FsArtifact? artifact, ArtifactActionResult artifactActionResult)
    {
        var result = await _artifactSelectionModalRef!.ShowAsync(artifact, artifactActionResult);
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
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
    }

    public async Task HandlePinArtifactsAsync(FsArtifact[] artifacts)
    {
        try
        {
            await PinService.SetArtifactsPinAsync(artifacts);
            await UpdatePinedArtifactsAsync(artifacts, true);
        }
        catch
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(Title, message, FxToastType.Error);
        }
    }

    public async Task HandleUnPinArtifactsAsync(FsArtifact[] artifacts)
    {
        try
        {
            var pathArtifacts = artifacts.Select(a => a.FullPath).ToArray();
            await PinService.SetArtifactsUnPinAsync(pathArtifacts);
            await UpdatePinedArtifactsAsync(artifacts, false);
        }
        catch
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(Title, message, FxToastType.Error);
        }
    }

    public async Task HandleDeleteArtifactsAsync(FsArtifact[] artifacts)
    {
        try
        {
            if (_confirmationModalRef != null)
            {
                var result = new ConfirmationModalResult();

                if (artifacts.Length == 1)
                {
                    var singleArtifact = artifacts.SingleOrDefault();
                    result = await _confirmationModalRef.ShowAsync(Localizer.GetString(AppStrings.DeleteItems, singleArtifact?.Name), Localizer.GetString(AppStrings.DeleteItemDescription));
                }
                else
                {
                    result = await _confirmationModalRef.ShowAsync(Localizer.GetString(AppStrings.DeleteItems, artifacts.Length), Localizer.GetString(AppStrings.DeleteItemsDescription));
                }

                if (result.ResultType == ConfirmationModalResultType.Confirm)
                {
                    await FileService.DeleteArtifactsAsync(artifacts);
                    await UpdateRemovedArtifactsAsync(artifacts);
                }
            }
        }
        catch (CanNotModifyOrDeleteDriveException ex)
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            _toastModalRef!.Show(Title, ex.Message, FxToastType.Error);
        }
        catch
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(Title, message, FxToastType.Error);
        }
    }

    public async Task HandleShowDetailsArtifact(FsArtifact[] artifact)
    {
        var isMultiple = artifact.Length > 1 ? true : false;
        var result = await _artifactDetailModalRef!.ShowAsync(artifact, isMultiple);
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
            case ArtifactDetailModalResultType.More:
                if (artifact.Length > 1)
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

        try
        {
            if (result?.ResultType == InputModalResultType.Confirm)
            {
                var newFolder = await FileService.CreateFolderAsync(path, result?.ResultName); //ToDo: Make CreateFolderAsync nullable
                _allArtifacts.Add(newFolder);   //Ugly, but no other possible way for now.
                FilterArtifacts();
            }
        }
        catch (DomainLogicException ex) when (ex is ArtifactNameNullException or ArtifactInvalidNameException or ArtifactAlreadyExistsException)
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = ex.Message;
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Error);
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

    private async Task LoadChildrenArtifactsAsync(FsArtifact? parentArtifact = null)
    {
        var allFiles = FileService.GetArtifactsAsync(parentArtifact?.FullPath);
        try
        {
            var artifacts = new List<FsArtifact>();
            await foreach (var item in allFiles)
            {
                item.IsPinned = PinService.IsPinned(item);
                artifacts.Add(item);
            }

            _allArtifacts = artifacts;
            FilterArtifacts();
        }
        //ToDo: Needs more business-wise data to implement
        catch (AndroidSpecialFilesUnauthorizedAccessException ex)
        {
            _toastModalRef!.Show(ex.Source, ex.Message, FxToastType.Error);
            _currentArtifact = await FileService.GetFsArtifactAsync(parentArtifact?.ParentFullPath);
        }
        //ToDo: Add a general catch in case of other exceptions
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
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(artifact.FullPath)
            });
#endif
        }
        else
        {
            _currentArtifact = artifact;
            await LoadChildrenArtifactsAsync(_currentArtifact);
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
        }

        switch (result?.ResultType)
        {
            case ArtifactOverflowResultType.Details:
                await HandleShowDetailsArtifact(new FsArtifact[] { artifact });
                break;
            case ArtifactOverflowResultType.Rename:
                await HandleRenameArtifactAsync(artifact);
                break;
            case ArtifactOverflowResultType.Copy:
                await HandleCopyArtifactsAsync(new FsArtifact[] { artifact });
                break;
            case ArtifactOverflowResultType.Pin:
                await HandlePinArtifactsAsync(new FsArtifact[] { artifact });
                break;
            case ArtifactOverflowResultType.UnPin:
                await HandleUnPinArtifactsAsync(new FsArtifact[] { artifact });
                break;
            case ArtifactOverflowResultType.Move:
                await HandleMoveArtifactsAsync(new FsArtifact[] { artifact });
                break;
            case ArtifactOverflowResultType.Delete:
                await HandleDeleteArtifactsAsync(new FsArtifact[] { artifact });
                break;
        }
    }

    public void ToggleSelectedAll()
    {
        if (_artifactExplorerMode == ArtifactExplorerMode.Normal)
        {
            _artifactExplorerMode = ArtifactExplorerMode.SelectArtifact;
            foreach (var artifact in _allArtifacts)
            {

            }
            _selectedArtifacts = _allArtifacts.ToArray();
            _isSelected = true;
        }
    }

    public void ChangeViewMode(ViewModeEnum mode)
    {
        _viewMode = mode;
    }

    public void CancelSelectionMode()
    {
        _artifactExplorerMode = ArtifactExplorerMode.Normal;
        _selectedArtifacts = Array.Empty<FsArtifact>();
        _isSelected = false;
    }

    private async Task HandleSelectedArtifactsOptions(FsArtifact[] artifacts)
    {
        var selectedArtifactsCount = artifacts.Length;
        var isMultiple = selectedArtifactsCount > 1;

        if (selectedArtifactsCount > 0)
        {
            ArtifactOverflowResult? result = null;
            if (_artifactOverflowModalRef is not null)
            {
                ArtifactExplorerModeChange(ArtifactExplorerMode.SelectArtifact);
                var pinOptionResult = GetPinOptionResult(artifacts);
                result = await _artifactOverflowModalRef!.ShowAsync(isMultiple, pinOptionResult);
            }

            switch (result?.ResultType)
            {
                case ArtifactOverflowResultType.Details:
                    await HandleShowDetailsArtifact(artifacts);
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
                    ArtifactExplorerModeChange(ArtifactExplorerMode.Normal);
                    break;
            }

            ArtifactExplorerModeChange(ArtifactExplorerMode.Normal);
        }
    }

    private void ArtifactExplorerModeChange(ArtifactExplorerMode mode)
    {
        _artifactExplorerMode = mode;
        _isSelected = false;
        StateHasChanged();
    }

    private PinOptionResult GetPinOptionResult(FsArtifact[] artifacts)
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
            artifactRenamed = _filteredArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
        }

        if (artifactRenamed != null)
        {
            var artifactParentPath = Path.GetDirectoryName(artifact.FullPath) ?? "";
            artifactRenamed.FullPath = Path.Combine(artifactParentPath, fullNewName);
            artifactRenamed.Name = fullNewName;
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
            foreach (var artifact in _filteredArtifacts)
            {
                if (artifactPath.Contains(artifact.FullPath))
                {
                    artifact.IsPinned = IsPinned;
                }
            }
            _allArtifacts = _filteredArtifacts;
        }

    }

    private async Task UpdateRemovedArtifactsAsync(IEnumerable<FsArtifact> artifacts)
    {
        if (artifacts.Count() == 1 && artifacts.SingleOrDefault()?.FullPath == _currentArtifact?.FullPath)
        {
            await HandleToolbarBackClick();
            return;
        }
        _filteredArtifacts = _filteredArtifacts.Except(artifacts).ToList();
        _allArtifacts = _filteredArtifacts;
    }

    private async Task HandleCancelSearchAsync()
    {
        _isInSearchMode = false;
        cancellationTokenSource?.Cancel();
        _searchText = string.Empty;
        await LoadChildrenArtifactsAsync(_currentArtifact);
    }

    private void HandleSearchFocused()
    {
        _isInSearchMode = true;
    }

    CancellationTokenSource? cancellationTokenSource;

    private async Task HandleDeepSearchAsync(string? text)
    {
        _searchText = text;
        _allArtifacts = new();
        FilterArtifacts();

        if (cancellationTokenSource is not null)
        {
            cancellationTokenSource.Cancel();
        }

        cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var sw = Stopwatch.StartNew();
        await Task.Run(async () =>
        {
            var buffer = new List<FsArtifact>();
            try
            {
                await foreach (var item in FileService.GetArtifactsAsync(_currentArtifact?.FullPath, _searchText, token))
                {
                    if (token.IsCancellationRequested)
                        break;

                    buffer.Add(item);
                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        if (token.IsCancellationRequested)
                            break;
                        _allArtifacts.AddRange(buffer);
                        FilterArtifacts();
                        buffer = new List<FsArtifact>();
                        sw.Restart();
                        await Task.Yield();
                    }
                }

                _allArtifacts.AddRange(buffer);
                FilterArtifacts();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

        });
    }

    private void HandleSearch(string? text)
    {
        if (text != null)
        {
            _searchText = text;
            _filteredArtifacts = _allArtifacts.Where(a => a.Name.ToUpper().Contains(text.ToUpper())).ToList();
        }
    }

    private async Task HandleToolbarBackClick()
    {
        _fxSearchInputRef?.HandleClearInputText();
        if (_artifactExplorerMode != ArtifactExplorerMode.Normal)
        {
            ArtifactExplorerModeChange(ArtifactExplorerMode.Normal);
        }
        if (!_isInSearchMode)
        {
            cancellationTokenSource?.Cancel();
            _fxSearchInputRef?.HandleClearInputText();
            await UpdateCurrentArtifactForBackButton(_currentArtifact);
            await LoadChildrenArtifactsAsync(_currentArtifact);
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("OnScrollEvent");
        }
        if (_isInSearchMode)
        {
            _isInSearchMode = false;
            _fxSearchInputRef?.HandleClearInputText();
            await LoadChildrenArtifactsAsync();
            StateHasChanged();
        }
    }

    private async Task UpdateCurrentArtifactForBackButton(FsArtifact fsArtifact)
    {
        if (fsArtifact.ParentFullPath is null)
        {
            _currentArtifact = null;
            return;
        }

        var previousArtifact = await FileService.GetFsArtifactAsync(fsArtifact?.ParentFullPath);
        if (previousArtifact != null && previousArtifact.ArtifactType == FsArtifactType.Drive)
        {
            var drives = FileService.GetArtifactsAsync(null);
            var rootArtifacts = new List<FsArtifact>();
            await foreach (var drive in drives)
            {
                rootArtifacts.Add(drive);
            }
            _currentArtifact = rootArtifacts.FirstOrDefault(a => a.FullPath == fsArtifact.ParentFullPath);
            return;
        }
        _currentArtifact = previousArtifact;
    }

    private void FilterArtifacts()
    {
        _filteredArtifacts = _allArtifacts;

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
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.LastModifiedDateTime).ToList();
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
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Size).ToList();
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
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.ArtifactType != FsArtifactType.Folder).ThenBy(artifact => artifact.Name).ToList();
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
        catch (DomainLogicException ex)
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            _toastModalRef!.Show(title, ex.Message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
    }

    private async Task RenameFolderAsync(FsArtifact? artifact, string? newName)
    {
        try
        {
            await FileService.RenameFolderAsync(artifact.FullPath, newName);
            UpdateRenamedArtifact(artifact, newName);
        }
        catch (DomainLogicException ex)
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            _toastModalRef!.Show(title, ex.Message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Error);
        }
    }

    private static List<FsArtifact> GetShouldOverwriteArtiacts(FsArtifact[] artifacts, List<FsArtifact> existArtifacts)
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
        _currentArtifact = await FileService.GetFsArtifactAsync(destinationPath);
        await LoadChildrenArtifactsAsync(_currentArtifact);
        await LoadPinsAsync();
    }
}