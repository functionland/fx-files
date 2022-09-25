
namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ConfirmationModal
    {
        private string Title { get; set; } = "Delete text2049-444.txt";
        private string Description { get; set; } = "Other users will lose access to this file. Are you sure you want to delete this file?";
        private bool _isModalOpen { get; set; }
        private TaskCompletionSource<ConfirmationModalResult>? _tcs = new();

        public async Task<ConfirmationModalResult> ShowAsync()
        {
            _tcs?.SetCanceled();

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ConfirmationModalResult>();

            return await _tcs.Task;
        }

        private void Confirm()
        {
            var result = new ConfirmationModalResult();

            result.ResultType = ConfirmationModalResultType.Confirm;

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        private void Close()
        {
            var result = new ConfirmationModalResult();

            result.ResultType = ConfirmationModalResultType.Cancel;

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Dispose()
        {
            _tcs?.SetCanceled();
        }
    }
}
