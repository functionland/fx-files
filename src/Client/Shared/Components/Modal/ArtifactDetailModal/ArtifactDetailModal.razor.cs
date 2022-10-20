using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ArtifactDetailModal
    {
        private List<FsArtifact> _artifacts = new();
        private string _artifactsSize = string.Empty;
        private int _currentArtifactForShowNumber = 0;
        private TaskCompletionSource<ArtifactDetailModalResult>? _tcs;
        private System.Timers.Timer? _timer;
        private bool _isModalOpen;
        private bool _isMultiple;
        private bool _isInRoot;

        [Parameter] public IFileService FileService { get; set; } = default!;

        public void Download()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Download;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Move()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Move;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Pin()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Pin;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void Unpin()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Unpin;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        public void More()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.More;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
        }

        //TODO: If we don't need to calculate the size of the artifacts for folder we can refactor this method
        public void CalculateArtifactsSize()
        {
            long? totalSize = 0;
            foreach (var artifact in _artifacts)
            {
                totalSize += artifact.Size;
            }

            _artifactsSize = FsArtifactUtils.CalculateSizeStr(totalSize);
        }

        public void ChangeArtifactSlideItem(bool isNext)
        {
            if (isNext)
            {
                _currentArtifactForShowNumber++;
            }
            else
            {
                _currentArtifactForShowNumber--;
            }
        }

        public async Task<ArtifactDetailModalResult> ShowAsync(List<FsArtifact> artifacts, bool isMultiple = false, bool isInRoot = false)
        {
            GoBackService.OnInit((Task () =>
            {
                Close();
                StateHasChanged();
                return Task.CompletedTask;
            }), true, false);

            _tcs?.SetCanceled();
            _currentArtifactForShowNumber = 0;
            _artifacts = artifacts;
            CalculateArtifactsSize();
            _isMultiple = isMultiple;
            _isInRoot = isInRoot;
            _isModalOpen = true;
            StateHasChanged();

            _tcs = new TaskCompletionSource<ArtifactDetailModalResult>();
            return await _tcs.Task;
        }

        private void Close()
        {
            var result = new ArtifactDetailModalResult();
            result.ResultType = ArtifactDetailModalResultType.Close;

            _tcs?.SetResult(result);
            _tcs = null;

            _isModalOpen = false;
            _timer = new(600);
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += async (sender, e) => { await TimeElapsedForCloseDetailModal(sender, e); };
        }

        private async Task TimeElapsedForCloseDetailModal(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _artifacts.Clear();
            await InvokeAsync(() =>
             {
                 StateHasChanged();
             });
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Stop();
            }
        }

        public void Dispose()
        {
            _tcs?.SetCanceled();
        }
    }
}
