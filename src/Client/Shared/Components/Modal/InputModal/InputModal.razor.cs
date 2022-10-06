namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class InputModal
    {
        [AutoInject]
        private IFileService _fileService = default!;

        private TaskCompletionSource<InputModalResult>? _tcs;

        private bool _isModalOpen;

        private string? _title;

        private string? _placeholder;

        private string? _inputValue;

        private string? _headTitle;


        public async Task<InputModalResult> ShowAsync(string tilte, string headTitle, string inputValue, string placeholder)
        {
            GoBackService.GoBackAsync = (Task () =>
            {
                Close();
                StateHasChanged();
                return Task.CompletedTask;
            });

            _headTitle = headTitle;
            _inputValue = inputValue;
            _title = tilte;
            _placeholder = placeholder;

            _tcs?.SetCanceled();
            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<InputModalResult>();
            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new InputModalResult();
            result.ResultType = InputModalResultType.Cancel;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        private void Confirm()
        {
            var result = new InputModalResult();
            result.ResultType = InputModalResultType.Confirm;
            result.ResultName = _inputValue;

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
