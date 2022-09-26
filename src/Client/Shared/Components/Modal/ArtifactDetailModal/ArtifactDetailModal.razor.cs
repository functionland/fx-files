using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ArtifactDetailModal
    {
        [AutoInject]
        private IFileService _fileService = default!;

        private List<FsArtifact> _artifacts = new();

        private string _artifactsSize = string.Empty;

        private TaskCompletionSource<ArtifactDetailModalResult>? _tcs;

        private bool _isModalOpen;

        public bool IsMultiple { get; set; }

        public void Download()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Download;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Move()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Move;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Pin()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Pin;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void More()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.More;

            _tcs!.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void CalculateArtifactsSize()
        {
            long? totalSize = 0;
            foreach (var artifact in _artifacts)
            {
                totalSize += artifact.Size;
            }

            _artifactsSize = FsArtifactUtils.CalculateSizeStr(totalSize);
        }

        public async Task<ArtifactDetailModalResult> ShowAsync(List<FsArtifact> artifacts, bool isMultiple = false)
        {
            _tcs?.SetCanceled();
            _artifacts = artifacts;
            CalculateArtifactsSize();
            IsMultiple = isMultiple;
            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ArtifactDetailModalResult>();
            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Close;

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
