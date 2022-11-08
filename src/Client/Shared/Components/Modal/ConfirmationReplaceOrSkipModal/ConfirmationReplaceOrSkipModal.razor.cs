namespace Functionland.FxFiles.Client.Shared.Components.Modal;

public partial class ConfirmationReplaceOrSkipModal
{
    private bool _isModalOpen;
    private TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>? _tcs;
    private int _artifactsCount;

    public async Task<ConfirmationReplaceOrSkipModalResult> ShowAsync(int artifactsCount)
    {
        GoBackService.OnInit((Task () =>
        {
            Close();
            StateHasChanged();
            return Task.CompletedTask;
        }), true, false);

        _artifactsCount = artifactsCount;
        _tcs?.SetCanceled();

        _isModalOpen = true;
        StateHasChanged();

        _tcs = new TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>();

        return await _tcs.Task;
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
