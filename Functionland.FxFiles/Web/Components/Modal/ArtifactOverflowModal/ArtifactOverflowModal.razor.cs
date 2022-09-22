namespace Functionland.FxFiles.App.Components.Modal
{
    public partial class ArtifactOverflowModal
    {
        private bool _isModalOpen;
        private TaskCompletionSource<ArtifactSelectionResult>? _tcs;
        [AutoInject]
        private IFileService _fileService = default!;
        private List<FsArtifact> _artifacts = new();

        [Parameter]
        public bool IsMultiple { get; set; }

        public async Task<ArtifactSelectionResult> ShowAsync()
        {
            _tcs?.SetCanceled();
            await LoadArtifacts();

            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ArtifactSelectionResult>();

            return await _tcs.Task;
        }

        private void SelectArtifact(FsArtifact artifact)
        {
            var result = new ArtifactSelectionResult();

            result.ResultType = ArtifactSelectionResultType.Ok;
            result.SelectedArtifacts = new[] { artifact };

            _tcs!.SetResult(result);
            _tcs = null;
            _isModalOpen = false;
        }

        private async Task LoadArtifacts()
        {
            var artifacts = _fileService.GetArtifactsAsync();

            await foreach (var item in artifacts)
            {
                if (item.ArtifactType != FsArtifactType.File)
                {
                    _artifacts.Add(item);
                }
            }
        }

        private void Close()
        {
            var result = new ArtifactSelectionResult();

            result.ResultType = ArtifactSelectionResultType.Cancel;

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
