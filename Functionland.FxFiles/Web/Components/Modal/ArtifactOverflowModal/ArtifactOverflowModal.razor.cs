namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ArtifactOverflowModal
    {
        [AutoInject]
        private IFileService _fileService = default!;
        private List<FsArtifact> _artifacts = new();
        private TaskCompletionSource<ArtifactOverflowResult>? _tcs;
        private bool _isModalOpen;

        [Parameter]
        public bool IsMultiple { get; set; }

        public void ShowDetails()
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Details;
            //result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Rename(FsArtifact artifact)
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Rename;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Copy(FsArtifact artifact)
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Copy;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Pin(FsArtifact artifact)
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Pin;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Move(FsArtifact artifact)
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Move;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public void Delete(FsArtifact artifact)
        {
            var result = new ArtifactOverflowResult();

            result.ResultType = ArtifactOverflowResultType.Details;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        public async Task<ArtifactOverflowResult> ShowAsync()
        {
            _tcs?.SetCanceled();

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
