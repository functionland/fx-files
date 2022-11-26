
namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ConfirmationModal
    {
        private string? _title;
        private string? _description;
        private bool _isModalOpen;
        private TaskCompletionSource<ConfirmationModalResult>? _tcs = new();

        public async Task<ConfirmationModalResult> ShowAsync(string title, string description)
        {
            GoBackService.SetState((Task () =>
            {
                Close();
                StateHasChanged();
                return Task.CompletedTask;
            }), true, false);

            _title = title;
            _description = description;

            _tcs?.SetCanceled();

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ConfirmationModalResult>();
            var result = await _tcs.Task;

            GoBackService.ResetToPreviousState();

            return result;
        }

        private void Confirm()
        {
            var result = new ConfirmationModalResult();

            result.ResultType = ConfirmationModalResultType.Confirm;

            _tcs?.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        private void Close()
        {
            var result = new ConfirmationModalResult();

            result.ResultType = ConfirmationModalResultType.Cancel;

            _tcs?.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Dispose()
        {
            _tcs?.SetCanceled();
        }
    }
}
