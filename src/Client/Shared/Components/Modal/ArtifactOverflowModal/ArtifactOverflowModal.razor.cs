namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ArtifactOverflowModal
    {
        private TaskCompletionSource<ArtifactOverflowResult>? _tcs;
        private bool _isModalOpen;
        private bool _isMultiple;
        private bool _isInRoot;
        private PinOptionResult? _pinOptionResult;
        private bool _isVisibleShareWithAppOption;
        private FileCategoryType? _fileCategoryType;

        public void Details()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Details;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        //TODO: complete extract onclick
        public void Extract()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Extract;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Rename()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Rename;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Copy()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Copy;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Pin()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Pin;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void UnPin()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.UnPin;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Move()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Move;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void ShareWithApp()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.ShareWithApp;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Delete()
        {
            var result = new ArtifactOverflowResult();
            result.ResultType = ArtifactOverflowResultType.Delete;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public async Task<ArtifactOverflowResult> ShowAsync(bool isMultiple, PinOptionResult pinOptionResult, bool isVisibleShareWithAppOption, FileCategoryType? fileCategoryType = null, bool isInRoot = false)
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
            _pinOptionResult = pinOptionResult;
            _isVisibleShareWithAppOption = isVisibleShareWithAppOption;
            _isModalOpen = true;
            _fileCategoryType = fileCategoryType;
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