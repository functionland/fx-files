using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class FilterArtifactModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<FileCategoryType?>? _tcs;

    [Parameter] public FileCategoryType? CurrentFilter { get; set; }

    public async Task<FileCategoryType?> ShowAsync()
    {
        GoBackService.SetState((Task () =>
        {
            HandleClose();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        _tcs?.SetCanceled();

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<FileCategoryType?>();
        FileCategoryType? result;

        try
        {
            result = await _tcs.Task;
        }
        catch (TaskCanceledException)
        {
            result = null;
        }

        GoBackService.ResetToPreviousState();

        return result;
    }

    private void HandleFilterItemClick(FileCategoryType? fileCategoryType)
    {
        _tcs?.SetResult(fileCategoryType);
        _tcs = null;
        _isModalOpen = false;
    }

    private void HandleClose()
    {
        _tcs?.SetResult(CurrentFilter);
        _tcs = null;
        _isModalOpen = false;
    }
}
