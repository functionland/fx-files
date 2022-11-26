namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ConfirmationReplaceOrSkipModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>? _tcs;
    private FsArtifact? _artifact;

    public async Task<ConfirmationReplaceOrSkipModalResult> ShowAsync(FsArtifact? artifact)
    {
        GoBackService.SetState((Task () =>
        {
            Close();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        _artifact = artifact;
        _tcs?.SetCanceled();

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>();
        var result = await _tcs.Task;

        GoBackService.ResetPreviousState();

        return result;
    }

    public void SkipArtifact()
    {
        var result = new ConfirmationReplaceOrSkipModalResult
        {
            ResultType = ConfirmationReplaceOrSkipModalResultType.Skip
        };

        try
        {
            _tcs?.SetResult(result);
        }
        finally
        {
            _tcs = null;
            _isModalOpen = false;
        }
    }

    public void ReplaceArtifact()
    {
        var result = new ConfirmationReplaceOrSkipModalResult
        {
            ResultType = ConfirmationReplaceOrSkipModalResultType.Replace
        };

        try
        {
            _tcs?.SetResult(result);
        }
        finally
        {
            _tcs = null;
            _isModalOpen = false;
        }
    }

    private void Close()
    {
        var result = new ConfirmationReplaceOrSkipModalResult
        {
            ResultType = ConfirmationReplaceOrSkipModalResultType.Skip
        };

        try
        {
            _tcs?.SetResult(result);
        }
        finally
        {
            _tcs = null;
            _isModalOpen = false;
        }

    }


    public void Dispose()
    {
        _tcs?.SetCanceled();
    }
}
