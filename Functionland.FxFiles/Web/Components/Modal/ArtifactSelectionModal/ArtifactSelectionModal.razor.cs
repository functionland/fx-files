namespace Functionland.FxFiles.App.Components.Modal;

public partial class ArtifactSelectionModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<ArtifactSelectionResult>? _tsc;

    [Parameter]
    public bool IsMultiple { get; set; }

    public async Task<ArtifactSelectionResult> ShowAsync()
    {
        _tsc?.SetCanceled();

        _isModalOpen = true;
        StateHasChanged();

        _tsc = new TaskCompletionSource<ArtifactSelectionResult>();

        return await _tsc.Task;
    }

    private void SelectArtifact(FsArtifact artifact)
    {
        var result = new ArtifactSelectionResult();

        result.ResultType = ArtifactSelectionResultType.Ok;
        result.SelectedArtifacts = new[] { artifact };

        _tsc!.SetResult(result);
        _isModalOpen = false;
    }

    private void Close()
    {
        var result = new ArtifactSelectionResult();

        result.ResultType = ArtifactSelectionResultType.Cancel;

        _tsc!.SetResult(result);
        _isModalOpen = false;
    }


    public void Dispose()
    {
        _tsc?.SetCanceled();
    }
}