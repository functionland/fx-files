namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ArtifactOverflowModal
    {
        private TaskCompletionSource<ArtifactOverflowResult>? _tcs;
        private bool _isModalOpen;
        private bool _isMultiple;
        private bool _isInRoot;
        private bool _isInFileViewer;
        private bool _isInSearch;
        private PinOptionResult? _pinOptionResult;
        private FileCategoryType? _fileCategoryType;
        private FsArtifactType? _fsArtifactType;

        public void Details()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Details;

            _tcs?.SetResult(result);
            _tcs = null;
            Close();
        }

        public void Extract()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Extract;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void OpenWith()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.OpenFileWithApp;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void Rename()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Rename;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void Copy()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Copy;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void Pin()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Pin;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void UnPin()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.UnPin;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void Move()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Move;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void ShareWithApp()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.ShareWithApp;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void ShowInLocation()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.ShowInLocation;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public void Delete()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Delete;

            _tcs?.SetResult(result);
            _tcs = null;

            Close();
        }

        public async Task<ArtifactOverflowResult> ShowAsync
            (bool isMultiple,
            PinOptionResult pinOptionResult,
            bool isInRoot,
            FileCategoryType? fileCategoryType = null,
            FsArtifactType? fsArtifactType = null,
            bool isInSearch = false,
            bool isInFileViewer = false)
        {
            GoBackService.OnInit((Task () =>
            {
                Close();
                StateHasChanged();
                return Task.CompletedTask;
            }), true, false);

            _tcs?.SetCanceled();
            _isInRoot = isInRoot;
            _isMultiple = isMultiple;
            _isInSearch = isInSearch;
            _pinOptionResult = pinOptionResult;
            _fileCategoryType = fileCategoryType;
            _fsArtifactType = fsArtifactType;
            _isInFileViewer = isInFileViewer;

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ArtifactOverflowResult>();
            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Cancel;

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