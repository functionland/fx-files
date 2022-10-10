using System.IO;

using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Services.Contracts;

namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ArtifactSelectionModal
{
    [AutoInject] private IFileService _fileService = default!;

    private bool _isModalOpen;
    private TaskCompletionSource<ArtifactSelectionResult>? _tcs;
    private List<FsArtifact> _artifacts = new();
    private FsArtifact? _currentArtifact;
    private ArtifactActionResult? _artifactActionResult;
    private InputModal _inputModalRef = default!;

    [Parameter] public bool IsMultiple { get; set; }

    public async Task<ArtifactSelectionResult> ShowAsync(FsArtifact? artifact, ArtifactActionResult artifactActionResult)
    {
        GoBackService.GoBackAsync = (Task () =>
        {
            Close();
            StateHasChanged();
            return Task.CompletedTask;
        });

        _tcs?.SetCanceled();
        _currentArtifact = artifact;
        _artifactActionResult = artifactActionResult;
        await LoadArtifacts(artifact?.FullPath);

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<ArtifactSelectionResult>();

        return await _tcs.Task;
    }
    private async Task SelectArtifact(FsArtifact artifact)
    {
        _currentArtifact = artifact;
        await LoadArtifacts(artifact.FullPath);
        StateHasChanged();
    }

    private void SelectDestionation()
    {
        try
        {
            if (_currentArtifact is null)
            {
                return;
            }

            var result = new ArtifactSelectionResult();

            result.ResultType = ArtifactSelectionResultType.Ok;
            result.SelectedArtifacts = new[] { _currentArtifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }
        catch (Exception)
        {

            throw;
        }
    }

    private async Task LoadArtifacts(string? path)
    {
        _artifacts = new List<FsArtifact>();
        var artifacts = _fileService.GetArtifactsAsync(path);
        var artifactPaths = _artifactActionResult?.Artifacts?.Select(a => a.FullPath);

        await foreach (var item in artifacts)
        {
            if (item.ArtifactType == FsArtifactType.File || (artifactPaths != null && artifactPaths.Contains(item.FullPath)))
            {
                item.IsDisabled = true;
            }

            _artifacts.Add(item);
        }
    }

    //TODO: Move to service and use in ArtifactExplorer
    private async Task CreateFolder()
    {
        if (_inputModalRef is null) return;

        var createFolder = Localizer.GetString(AppStrings.CreateFolder);
        var newFolderPlaceholder = Localizer.GetString(AppStrings.NewFolderPlaceholder);

        var result = await _inputModalRef.ShowAsync(createFolder, string.Empty, string.Empty, newFolderPlaceholder);

        try
        {
            if (result?.ResultType == InputModalResultType.Confirm)
            {
                var newFolder = await _fileService.CreateFolderAsync(_currentArtifact.FullPath, result?.ResultName); //ToDo: Make CreateFolderAsync nullable
                _artifacts.Add(newFolder);
                StateHasChanged();
            }
        }
        catch (Exception exception)
        {
            ExceptionHandler?.Handle(exception);
        }
    }

    private async Task Back()
    {
        try
        {
            _currentArtifact = await _fileService.GetArtifactAsync(_currentArtifact?.ParentFullPath);
        }
        catch (DomainLogicException ex) when (ex is ArtifactPathNullException)
        {
            _currentArtifact = null;
        }
        
        await LoadArtifacts(_currentArtifact?.FullPath);
    }

    private void Close()
    {
        var result = new ArtifactSelectionResult();

        result.ResultType = ArtifactSelectionResultType.Cancel;

        _tcs!.SetResult(result);
        _tcs = null;
        _isModalOpen = false;
    }

    public void Dispose()
    {
        _tcs?.SetCanceled();
    }
}