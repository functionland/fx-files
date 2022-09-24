namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ArtifactOverflowModal
    {
        [AutoInject]
        private IFileService _fileService = default!;

        private TaskCompletionSource<ArtifactOverflowResult>? _tcs;

        private bool _isModalOpen;

        public bool IsMultiple { get; set; }

        public void Details()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Details;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Rename()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Rename;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Copy()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Copy;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Pin()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Pin;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Move()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Move;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Delete()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Delete;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public async Task<ArtifactOverflowResult> ShowAsync(bool isMultiple = false)
        {
            _tcs?.SetCanceled();
            IsMultiple = isMultiple;
            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ArtifactOverflowResult>();
            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Cancel;

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