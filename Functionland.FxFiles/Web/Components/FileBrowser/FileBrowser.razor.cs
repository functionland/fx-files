using Functionland.FxFiles.App.Components.Common;
using Functionland.FxFiles.App.Components.DesignSystem;
using Functionland.FxFiles.App.Components.Modal;
using Functionland.FxFiles.Shared.Services.Contracts;

using Microsoft.Extensions.Localization;

namespace Functionland.FxFiles.App.Components;

public partial class FileBrowser
{
    private FsArtifact? _currentArtifact;

    private List<FsArtifact> _pins = new();

    private List<FsArtifact> _artifacts = new();

    private ArtifactSelectionModal? _artifactSelectionModalRef { get; set; }
    private ArtifactOverflowModal _asm { get; set; }

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
        List<FsArtifact> existArtifacts = new();
        string? destinationPath = await HandleSelectDestinationArtifact();
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
            //TODO: handle skip or replace artifactsToCopy
        }
    }

    public async Task HandleMoveArtifactsAsync(List<FsArtifact> artifacts)
    {
        List<FsArtifact> existArtifacts = new();
        string? destinationPath = await HandleSelectDestinationArtifact();
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
            //TODO: handle skip or replace artifactsToMove
        }

    }

    public async Task<string?> HandleSelectDestinationArtifact()
    {
        var Result = await _artifactSelectionModalRef?.ShowAsync();
        string? destinationPath = null;

        if (Result?.ResultType == ArtifactSelectionResultType.Ok)
        {
            var destinationFsArtifact = Result.SelectedArtifacts.FirstOrDefault();
            destinationPath = destinationFsArtifact?.FullPath;
        }

        return destinationPath;
    }

    public async Task HandleRenameArtifact(FsArtifact artifact, string newName)
    {
        string filePath = artifact.FullPath;
        try
        {
            await FileService.RenameFileAsync(filePath, newName);
        }
        catch (DomainLogicException ex) when (ex.Message == Localizer.GetString(AppStrings.ArtifactPathIsNull, "file"))
        {
            //TODO: show exeception message with toast
        }
    }

    public async Task HandlePinArtifacts(List<FsArtifact> artifact)
    {

    }

    public async Task HandleDeleteArtifacts(List<FsArtifact> artifact)
    {

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
        var allPins = PinService.GetPinnedArtifactsAsync(null);

        var pins = new List<FsArtifact>();

        await foreach (var item in allPins)
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

        switch(result.ResultType)
        {
            case ArtifactOverflowResultType.Details:
                await HandleShowDetailsArtifact(new List<FsArtifact>() { artifact });
                break;
            case ArtifactOverflowResultType.Rename:
                await HandleRenameArtifact(artifact, artifact.Name);
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

        if(selectedArtifactsCount > 0)
        {
            var result = await _asm.ShowAsync(isMultiple);

            switch (result.ResultType)
            {
                case ArtifactOverflowResultType.Details:
                    await HandleShowDetailsArtifact(artifacts);
                    break;
                case ArtifactOverflowResultType.Rename:
                    var singleArtifact = artifacts.SingleOrDefault();
                    await HandleRenameArtifact(singleArtifact, singleArtifact.Name);
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

    private bool IsInRoot(FsArtifact? artifact)
    {
        return artifact is null ? true : false;
    }
}