namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ConfirmationReplaceOrSkipModal
    {
        private bool _isModalOpen;
        private TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>? _tcs;

        [Parameter]
        public string[] ArtifactsName { get; set; } = default!;

        public async Task<ConfirmationReplaceOrSkipModalResult> ShowAsync()
        {
            _tcs?.SetCanceled();

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>();

            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new ConfirmationReplaceOrSkipModalResult();

            result.ResultType = ConfirmationReplaceOrSkipModalResultType.Skip;

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
