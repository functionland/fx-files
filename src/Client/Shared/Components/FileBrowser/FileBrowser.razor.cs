using Functionland.FxFiles.Client.Shared.Components.Common;
using Functionland.FxFiles.Client.Shared.Components.Modal;
using Functionland.FxFiles.Client.Shared.Models;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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

    private string? _searchText;
    private bool _isInSearchMode;
    private FileCategoryType? _fileCategoryFilter;
    private SortTypeEnum _currentSortType = SortTypeEnum.Name;
    private bool _IsAscOrder = true;

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
                Count = artifacts.Length,
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

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef != null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts);
                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.CopyArtifactsAsync(existArtifacts.ToArray(), destinationPath, true);
                    }
                }
            }

            var title = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Success);
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
                Count = artifacts.Length,
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

            var movedArtifact = artifacts.Except(existArtifacts);
            UpdateRemovedArtifacts(movedArtifact);

            if (existArtifacts.Count > 0)
            {
                if (_confirmationReplaceOrSkipModalRef is not null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(existArtifacts);
                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.MoveArtifactsAsync(existArtifacts.ToArray(), destinationPath, true);
                        UpdateRemovedArtifacts(existArtifacts);
                    }
                }
            }

            var title = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedMessage);
            _toastModalRef!.Show(title, message, FxToastType.Success);
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
        var Result = await _artifactSelectionModalRef!.ShowAsync(artifact, artifactActionResult);
        string? destinationPath = null;

        if (Result?.ResultType == ArtifactSelectionResultType.Ok)
        {
            var destinationFsArtifact = Result.SelectedArtifacts.FirstOrDefault();
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

        try
        {
            if (artifact?.ArtifactType == FsArtifactType.Folder)
            {
                await FileService.RenameFolderAsync(artifact.FullPath, newName);
                UpdateRenamedArtifact(artifact, newName);
            }
            else if (artifact?.ArtifactType == FsArtifactType.File)
            {
                await FileService.RenameFileAsync(artifact.FullPath, newName);
                var artifactRenamed = _allArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
                UpdateRenamedArtifact(artifact, newName);
            }
            else if (artifact?.ArtifactType == FsArtifactType.Drive)
            {
                var title = Localizer.GetString(AppStrings.ToastErrorTitle);
                var message = Localizer.GetString(AppStrings.RootfolderRenameException);
                _toastModalRef!.Show(title, message, FxToastType.Error);
            }
        }
        catch (DomainLogicException ex) when
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameIsNull, artifact?.ArtifactType.ToString() ?? "") ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameHasInvalidChars, artifact?.ArtifactType.ToString() ?? "")) ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactAlreadyExistsException, artifact?.ArtifactType.ToString() ?? "")))
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
                    UpdateRemovedArtifacts(artifacts);
                }
            }
        }
        catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.DriveRemoveFailed))
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.RootFolderDeleteException);
            _toastModalRef!.Show(Title, message, FxToastType.Error);
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
                //TODO: Implement more here 
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
        catch (DomainLogicException ex) when
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameIsNull, "folder") ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder") ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactAlreadyExistsException, "folder"))))
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
        var artifacts = new List<FsArtifact>();
        await foreach (var item in allFiles)
        {
            item.IsPinned = PinService.IsPinned(item);
            artifacts.Add(item);
        }

        _allArtifacts = artifacts;
        FilterArtifacts();
    }

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }

    private async Task HandleSelectArtifactAsync(FsArtifact artifact)
    {
        _currentArtifact = artifact;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        // load current artifacts
    }

    private async Task HandleOptionsArtifact(FsArtifact artifact)
    {
        ArtifactOverflowResult? result = null;
        if (_artifactOverflowModalRef is not null)
        {
            var isPinned = artifact.IsPinned ?? false;
            result = await _artifactOverflowModalRef!.ShowAsync(false, isPinned);
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

    private async Task HandleSelectedArtifactsOptions(FsArtifact[] artifacts)
    {
        var selectedArtifactsCount = artifacts.Length;
        var isMultiple = selectedArtifactsCount > 1;

        if (selectedArtifactsCount > 0)
        {
            ArtifactOverflowResult? result = null;
            if (_artifactOverflowModalRef is not null)
            {
                result = await _artifactOverflowModalRef!.ShowAsync(isMultiple);
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
                case ArtifactOverflowResultType.Move:
                    await HandleMoveArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Delete:
                    await HandleDeleteArtifactsAsync(artifacts);
                    break;
            }
        }
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

    private void UpdateRenamedArtifact(FsArtifact artifact, string newName)
    {
        var artifactRenamed = _filteredArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
        if (artifactRenamed != null)
        {
            var artifactParentPath = Path.GetDirectoryName(artifact.FullPath) ?? "";
            artifactRenamed.FullPath = Path.Combine(artifactParentPath, newName);
            artifactRenamed.Name = newName + Path.GetExtension(artifact.Name);
        }

        _allArtifacts = _filteredArtifacts;
    }

    private async Task UpdatePinedArtifactsAsync(IEnumerable<FsArtifact> artifacts, bool IsPinned)
    {
        await LoadPinsAsync();
        var artifactPath = artifacts.Select(a => a.FullPath);

        foreach (var artifact in _filteredArtifacts)
        {
            if (artifactPath.Contains(artifact.FullPath))
            {
                artifact.IsPinned = IsPinned;
            }
        }

        _allArtifacts = _filteredArtifacts;
    }

    private void UpdateRemovedArtifacts(IEnumerable<FsArtifact> artifacts)
    {
        _filteredArtifacts = _filteredArtifacts.Except(artifacts).ToList();
        _allArtifacts = _filteredArtifacts;
    }

    private void HandleSearchFocused()
    {
        _isInSearchMode = true;
    }

    private async Task HandleSearch(string? text)
    {
        //await InvokeAsync(async () =>
        //{
        _searchText = text;
        _allArtifacts = new();

        var result = FileService.GetArtifactsAsync(_currentArtifact?.FullPath, _searchText);
        await foreach (var item in result)
        {
            _allArtifacts.Add(item);
        }

        FilterArtifacts();
        //});
    }

    private async Task HandleToolbarBackClick()
    {
        _isInSearchMode = false;
        _searchText = string.Empty;
        _currentArtifact = _currentArtifact?.ParentFullPath is null ? null : await FileService.GetFsArtifactAsync(_currentArtifact?.ParentFullPath);
        await LoadChildrenArtifactsAsync(_currentArtifact);
        FilterArtifacts();
        StateHasChanged();
    }

    private void FilterArtifacts()
    {
        //_filteredArtifacts = string.IsNullOrWhiteSpace(_searchText)
        //? _allArtifacts
        //    : _allArtifacts.Where(a => a.Name.Contains(_searchText)).ToList();

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
        _IsAscOrder = !_IsAscOrder;
        sortFilteredArtifacts();
    }

    private async Task HandleSortClick()
    {
        _currentSortType = await _sortedArtifactModalRef!.ShowAsync();
        sortFilteredArtifacts();
    }

    private void sortFilteredArtifacts()
    {
        if (_currentSortType is SortTypeEnum.LastModified)
        {
            if (_IsAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.LastModifiedDateTime).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.LastModifiedDateTime).ToList();
                return;
            }

        }

        if (_currentSortType is SortTypeEnum.Size)
        {
            if (_IsAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.Size).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.Size).ToList();
                return;
            }
        }

        if (_currentSortType is SortTypeEnum.Name)
        {
            if (_IsAscOrder)
            {
                _filteredArtifacts = _filteredArtifacts.OrderBy(artifact => artifact.Name).ToList();
                return;
            }
            else
            {
                _filteredArtifacts = _filteredArtifacts.OrderByDescending(artifact => artifact.Name).ToList();
                return;
            }
        }
    }
}