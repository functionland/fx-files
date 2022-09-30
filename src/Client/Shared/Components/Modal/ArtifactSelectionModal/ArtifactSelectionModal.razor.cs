namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ArtifactSelectionModal
{
    [AutoInject] private IFileService _fileService = default!;

    private bool _isModalOpen;
    private TaskCompletionSource<ArtifactSelectionResult>? _tcs;
    private List<FsArtifact> _artifacts = new();
    private FsArtifact? _currentArtifact;
    private ArtifactActionResult? _artifactActionResult;

    [Parameter] public bool IsMultiple { get; set; }

    public async Task<ArtifactSelectionResult> ShowAsync(FsArtifact? artifact, ArtifactActionResult artifactActionResult)
    {
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

    private async Task LoadArtifacts(string? path)
    {
        _artifacts = new List<FsArtifact>();
        var artifacts = _fileService.GetArtifactsAsync(path);     
        var artifactPaths = _artifactActionResult?.Artifacts?.Select(a => a.FullPath);

        await foreach (var item in artifacts)
        {
            if (artifactPaths.Contains(item.FullPath)) continue;

            if (item.ArtifactType != FsArtifactType.File)
            {
                _artifacts.Add(item);
            }
        }
    }

    private async Task Back()
    {
        _currentArtifact = _currentArtifact?.ParentFullPath is null ? null : await _fileService.GetFsArtifactAsync(_currentArtifact?.ParentFullPath);
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