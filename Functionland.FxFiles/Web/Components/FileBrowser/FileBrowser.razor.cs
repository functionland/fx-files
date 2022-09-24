using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.DesignSystem;
using Functionland.FxFiles.App.Components.Modal;
using Functionland.FxFiles.Shared.Models;
using Functionland.FxFiles.Shared.Services.Contracts;

using Microsoft.Extensions.Localization;

namespace Functionland.FxFiles.App.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;

    private List<FsArtifact> _pins = new();

    private List<FsArtifact> _artifacts = new();

    private InputModal _inputModal { get; set; }
    private ArtifactOverflowModal _asm { get; set; }
    private ToastModal? _toastModalRef { get; set; }
    private ArtifactSelectionModal? _artifactSelectionModalRef { get; set; }
    private ConfirmationReplaceOrSkipModal? _ConfirmationReplaceOrSkipModalRef { get; set; }

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

            string? destinationPath = await HandleSelectDestinationArtifact(_currentArtifact, artifactActionResult);
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
                var Result = await _ConfirmationReplaceOrSkipModalRef.ShowAsync();
                if (Result.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                {
                    await FileService.CopyArtifactsAsync(existArtifacts.ToArray(), destinationPath, true);
                }
            }

            var Title = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheCopyOpreationSuccessedMessage);
            _toastModalRef?.Show(Title, message, FxToastType.Success);
        }
        catch
        {
            var Title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef?.Show(Title, message, FxToastType.Error);
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

            string? destinationPath = await HandleSelectDestinationArtifact(_currentArtifact, artifactActionResult);
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

            artifacts = artifacts.Except(existArtifacts).ToList();
            _artifacts = _artifacts.Except(artifacts).ToList();

            if (existArtifacts.Any())
            {
                if (_ConfirmationReplaceOrSkipModalRef is not null)
                {
                    var Result = await _ConfirmationReplaceOrSkipModalRef.ShowAsync();
                    if (Result.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                    {
                        await FileService.MoveArtifactsAsync(existArtifacts.ToArray(), destinationPath, true);
                        _artifacts = _artifacts.Except(existArtifacts).ToList();
                    }
                }
            }

            var title = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedTiltle);
            var message = Localizer.GetString(AppStrings.TheMoveOpreationSuccessedMessage);
            _toastModalRef?.Show(title, message, FxToastType.Success);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef?.Show(title, message, FxToastType.Error);
        }

    }

    public async Task<string?> HandleSelectDestinationArtifact(FsArtifact artifact, ArtifactActionResult artifactActionResult)
    {
        var Result = await _artifactSelectionModalRef?.ShowAsync(artifact, artifactActionResult);
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

        if (result == null || result.ResultType == InputModalResultType.Cancel)
        {
            return;
        }

        string? newName = "C";

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
                var artifactRenamed = _artifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
                UpdateRenamedArtifact(artifact, newName);
            }
            else
            {
                var title = Localizer.GetString(AppStrings.ToastErrorTitle);
                var message = Localizer.GetString(AppStrings.RootfolderRenameException);
                _toastModalRef?.Show(title, message, FxToastType.Error);
            }
        }
        catch (DomainLogicException ex) when
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameIsNull, artifact?.ArtifactType.ToString() ?? "") ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactNameHasInvalidChars, artifact?.ArtifactType.ToString() ?? "")) ||
        (ex.Message == Localizer.GetString(AppStrings.ArtifactAlreadyExistsException, artifact?.ArtifactType.ToString() ?? "")))
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = ex.Message;
            _toastModalRef?.Show(title, message, FxToastType.Error);
        }
        catch
        {
            var title = Localizer.GetString(AppStrings.ToastErrorTitle);
            var message = Localizer.GetString(AppStrings.TheOpreationFailedMessage);
            _toastModalRef?.Show(title, message, FxToastType.Error);
        }

    }

    public async Task HandlePinArtifacts(List<FsArtifact> artifacts)
    {
        var notPinedArtifacts = artifacts.Where(a => a.IsPinned == false).ToArray();

        await PinService.SetArtifactsPinAsync(notPinedArtifacts.ToArray());
    }

    public async Task HandleDeleteArtifacts(List<FsArtifact> artifacts)
    {
        //TODO: if (cancellationToken?.IsCancellationRequested == true)
        foreach (var artifact in artifacts)
        {
            try
            {
                await FileService.DeleteArtifactsAsync(artifacts.ToArray());
            }

            catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.ArtifactPathIsNull, artifact?.ArtifactType.ToString() ?? ""))
            {
                // show exeception message with toast
            }

            catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.DriveRemoveFailed))
            {
                // show exeception message with toast
            }
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
            artifacts.Add(item);
        }

        _artifacts = artifacts;
    }

    private async Task HandleSelectArtifact(FsArtifact artifact)
    {
        _currentArtifact = artifact;
        await LoadChildrenArtifactsAsync(_currentArtifact);
        // load current artifacts
    }

    private async Task HandleOptionsArtifact(FsArtifact artifact)
    {
        var result = await _asm.ShowAsync();

        switch (result.ResultType)
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
            var result = await _asm.ShowAsync(isMultiple);

            switch (result.ResultType)
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
            artifactType = "File name";
        }
        else if (artifact.ArtifactType == FsArtifactType.Folder)
        {
            artifactType = "Folder name";
        }
        else
        {
            return null;
        }

        var Name = Path.GetFileNameWithoutExtension(artifact.Name);
        var result = await _inputModal.ShowAsync("Change name", Name, artifactType, true);

        return result;
    }

    private void UpdateRenamedArtifact(FsArtifact artifact, string newName)
    {
        var artifactRenamed = _artifacts.Where(a => a.FullPath == artifact.FullPath).FirstOrDefault();
        if (artifactRenamed != null)
        {
            var artifactParentPath = Path.GetDirectoryName(artifact.FullPath) ?? "";
            artifactRenamed.FullPath = Path.Combine(artifactParentPath, artifact.Name);
            artifactRenamed.Name = newName;
        }
    }

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }
}