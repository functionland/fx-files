using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;
    private List<FsArtifact> _pins = new();
    private List<FsArtifact> _allArtifacts = new();
    private List<FsArtifact> _filteredArtifacts = new();

    private InputModal? _inputModal;
    private ToastModal? _toastModalRef;
    private ConfirmationModal? _confirmationModalRef;
    private FilterArtifactModal? _filteredArtifactModalRef;
    private ArtifactOverflowModal? _artifactOverflowModalRef;
    private ArtifactSelectionModal? _artifactSelectionModalRef;
    private ConfirmationReplaceOrSkipModal? _confirmationReplaceOrSkipModalRef;

    private string? _searchText;
    private bool _isInSearchMode;
    private FileCategoryType? _fileCategoryFilter;

    [Parameter] public IPinService PinService { get; set; } = default!;

    [Parameter] public IFileService FileService { get; set; } = default!;

    protected override async Task OnInitAsync()
    {
        await LoadPinsAsync();

        await LoadChildrenArtifactsAsync();

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
                Count = artifacts.Count,
            };

            string? destinationPath = await HandleSelectDestinationAsync(_currentArtifact, artifactActionResult);
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                return;
            }

            try
            {
                await FileService.CopyArtifactsAsync(artifacts.ToArray(), destinationPath, false);
            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }

            if (existArtifacts.Any())
            {
                if (_confirmationReplaceOrSkipModalRef != null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(artifacts.ToArray());
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

    public async Task HandleMoveArtifactsAsync(List<FsArtifact> artifacts)
    {
        try
        {
            List<FsArtifact> existArtifacts = new();
            var artifactActionResult = new ArtifactActionResult()
            {
                ActionType = ArtifactActionType.Move,
                Count = artifacts.Count,
            };

            string? destinationPath = await HandleSelectDestinationAsync(_currentArtifact, artifactActionResult);
            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                return;
            }

            try
            {
                await FileService.MoveArtifactsAsync(artifacts.ToArray(), destinationPath, false);
            }
            catch (CanNotOperateOnFilesException ex)
            {
                existArtifacts = ex.FsArtifacts;
            }

            artifacts = artifacts.Except(existArtifacts).ToList();
            UpdateRemovedArtifacts(artifacts);

            if (existArtifacts.Any())
            {
                if (_confirmationReplaceOrSkipModalRef is not null)
                {
                    var result = await _confirmationReplaceOrSkipModalRef.ShowAsync(artifacts.ToArray());
                    if (result?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.MoveArtifactsAsync(existArtifacts.ToArray(), destinationPath, true);
                        UpdateRemovedArtifacts(existArtifacts);
                    }
                }
            }

            FilterArtifacts();

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

    public async Task HandleRenameArtifact(FsArtifact artifact)
    {

        var result = await GetInputModalResult(artifact);

        if (result?.ResultType == InputModalResultType.Cancel)
        {
            return;
        }

        string? newName = result?.ResultName;

        try
        {
            if (artifact.ArtifactType == FsArtifactType.Folder)
            {
                await FileService.RenameFolderAsync(artifact.FullPath, newName);
                UpdateRenamedArtifact(artifact, newName);
            }
            else if (artifact.ArtifactType == FsArtifactType.File)
            {
                await FileService.RenameFileAsync(artifact.FullPath, newName);
                var artifactRenamed = _allArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
                UpdateRenamedArtifact(artifact, newName);
            }
            else
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

    public async Task HandlePinArtifacts(List<FsArtifact> artifacts)
    {
        var notPinedArtifacts = artifacts.Where(a => a.IsPinned != true).ToArray();

        try
        {
            await PinService.SetArtifactsPinAsync(notPinedArtifacts);
            await UpdatePinedArtifactsAsync(notPinedArtifacts.ToList());
        }
        catch
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef!.Show(Title, message, FxToastType.Error);
        }
    }

    public async Task HandleDeleteArtifacts(List<FsArtifact> artifacts)
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
                }
                else
                {
                    result = await _confirmationModalRef.ShowAsync(Localizer.GetString(AppStrings.DeleteItems, artifacts.Count), Localizer.GetString(AppStrings.DeleteItemsDescription));
                }

                if (result.ResultType == ConfirmationModalResultType.Confirm)
                {
                    await FileService.DeleteArtifactsAsync(artifacts.ToArray());
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

    public async Task HandleShowDetailsArtifact(List<FsArtifact> artifact)
    {

    }

    public async Task HandleCreateFolder(string path, string folderName)
    {
        try
        {
            await FileService.CreateFolderAsync(path, folderName);
        }
        catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.ArtifactPathIsNull, "folder"))
        {
            // ToDo: toast something (...)
        }
        catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.ArtifactNameIsNull, "folder"))
        {
            // ToDo: toast something (Your folder needs a name, enter something.)
        }
        catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.ArtifactNameHasInvalidChars, "folder"))
        {
            // ToDo: toast something (Name has invalid characters. Think again about what you pick.)
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

    private async Task HandleSelectArtifact(FsArtifact artifact)
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
            result = await _artifactOverflowModalRef!.ShowAsync();
        }

        switch (result?.ResultType)
        {
            case ArtifactOverflowResultType.Details:
                await HandleShowDetailsArtifact(new List<FsArtifact>() { artifact });
                break;
            case ArtifactOverflowResultType.Rename:
                await HandleRenameArtifact(artifact);
                break;
            case ArtifactOverflowResultType.Copy:
                await HandleCopyArtifactsAsync(new List<FsArtifact>() { artifact });
                break;
            case ArtifactOverflowResultType.Pin:
                await HandlePinArtifacts(new List<FsArtifact>() { artifact });
                break;
            case ArtifactOverflowResultType.Move:
                await HandleMoveArtifactsAsync(new List<FsArtifact>() { artifact });
                break;
            case ArtifactOverflowResultType.Delete:
                await HandleDeleteArtifacts(new List<FsArtifact>() { artifact });
                break;
        }
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
                result = await _artifactOverflowModalRef!.ShowAsync();
            }

            switch (result?.ResultType)
            {
                case ArtifactOverflowResultType.Details:
                    await HandleShowDetailsArtifact(artifacts);
                    break;
                case ArtifactOverflowResultType.Rename:
                    var singleArtifact = artifacts.SingleOrDefault();
                    await HandleRenameArtifact(singleArtifact);
                    break;
                case ArtifactOverflowResultType.Copy:
                    await HandleCopyArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Pin:
                    await HandlePinArtifacts(artifacts);
                    break;
                case ArtifactOverflowResultType.Move:
                    await HandleMoveArtifactsAsync(artifacts);
                    break;
                case ArtifactOverflowResultType.Delete:
                    await HandleDeleteArtifacts(artifacts);
                    break;
            }
        }
    }

    private async Task<InputModalResult?> GetInputModalResult(FsArtifact artifact)
    {
        string artifactType = "";

        if (artifact.ArtifactType == FsArtifactType.File)
        {
            artifactType = Localizer.GetString(AppStrings.FileRenamePlaceholder);
        }
        else if (artifact.ArtifactType == FsArtifactType.Folder)
        {
            artifactType = Localizer.GetString(AppStrings.FolderRenamePlaceholder);
        }
        else
        {
            return null;
        }

        var Name = Path.GetFileNameWithoutExtension(artifact.Name);

        InputModalResult? result = null;
        if (_inputModal is not null)
        {
            result = await _inputModal.ShowAsync(Localizer.GetString(AppStrings.ChangeName), Localizer.GetString(AppStrings.Rename).ToString().ToUpper(), Name, artifactType) ;
        }

        return result;
    }

    private void UpdateRenamedArtifact(FsArtifact artifact, string newName)
    {
        var artifactRenamed = _filteredArtifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
        if (artifactRenamed != null)
        {
            var artifactParentPath = Path.GetDirectoryName(artifact.FullPath) ?? "";
            artifactRenamed.FullPath = Path.Combine(artifactParentPath, artifact.Name);
            artifactRenamed.Name = newName + Path.GetExtension(artifact.Name);
        }

        _allArtifacts = _filteredArtifacts;
    }

    private async Task UpdatePinedArtifactsAsync(List<FsArtifact> artifacts)
    {
        await LoadPinsAsync();
        var artifactPath = artifacts.Select(a => a.FullPath).ToList();

        foreach (var artifact in _filteredArtifacts)
        {
            if (artifactPath.Contains(artifact.FullPath))
            {
                artifact.IsPinned = true;
            }
        }

        _allArtifacts = _filteredArtifacts;
    }

    private void UpdateRemovedArtifacts(List<FsArtifact> artifacts)
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
}