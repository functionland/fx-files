using Functionland.FxFiles.Client.Shared.Components.Common;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class SortArtifactModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<SortTypeEnum>? _tcs;
    public SortTypeEnum x;

    [Parameter] public SortTypeEnum CurrentSort { get; set; }

    public async Task<SortTypeEnum> ShowAsync()
    {
        GoBackService.GoBackAsync = (Task () =>
        {
            HandleClose();
            StateHasChanged();
            return Task.CompletedTask;
        });

        _tcs?.SetCanceled();

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<SortTypeEnum>();

        return await _tcs.Task;
    }

    private void HandleSortItemClick(SortTypeEnum sortType)
    {
        _tcs!.SetResult(sortType);
        _tcs = null;
        _isModalOpen = false;
    }

    private void HandleClose()
    {
        _tcs!.SetResult(CurrentSort);
        _tcs = null;
        _isModalOpen = false;
    }
}
