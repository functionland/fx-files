namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ConfirmationReplaceOrSkipModal
    {
        private bool _isModalOpen;
        private TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>? _tcs;

        [Parameter]
        public string[] ArtifactsNames { get; set; } = default!;

        public async Task<ConfirmationReplaceOrSkipModalResult> ShowAsync(IEnumerable<FsArtifact> artifacts)
        {
            ArtifactsNames = artifacts.Select(a => a.Name).ToArray();

            _tcs?.SetCanceled();

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ConfirmationReplaceOrSkipModalResult>();

            return await _tcs.Task;
        }

        public void SkipArtifact()
        {
            var result = new ConfirmationReplaceOrSkipModalResult();

            result.ResultType = ConfirmationReplaceOrSkipModalResultType.Skip;

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void ReplaceArtifact()
        {
            var result = new ConfirmationReplaceOrSkipModalResult();

            result.ResultType = ConfirmationReplaceOrSkipModalResultType.Replace;

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
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
