using System.Diagnostics;
using Functionland.FxFiles.Client.Shared.Enums;
using Functionland.FxFiles.Client.Shared.Models;
using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ArtifactDetailModal
    {
        private List<FsArtifact> _artifacts = new();
        private string _artifactsSize = string.Empty;
        private int _currentArtifactForShowNumber = 0;
        private FsArtifact? _currentArtifact = null;
        private TaskCompletionSource<ArtifactDetailModalResult>? _tcs;
        private System.Timers.Timer? _timer;
        private bool _isModalOpen;
        private bool _isMultiple;
        private bool _isInRoot;

        //private string? _folderOrDriveSize;

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
        public async IAsyncEnumerable<long?> CalculateFolderOrDriveSize(string folderPath)
        {
            await foreach (var size in FileService.GetArtifactSizeAsync(folderPath))
            {
                yield return size;
            }
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

            SetCurrentArtifact(_currentArtifactForShowNumber);
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
            SetCurrentArtifact(0);
            _artifacts = artifacts;
            _isMultiple = isMultiple;
            _isInRoot = isInRoot;
            _isModalOpen = true;

            _artifactsSize = FsArtifactUtils.CalculateSizeStr(_artifacts.Sum(a => a.Size ?? 0));
            StateHasChanged();

            _ = UpdateArtifactSizesAsync();

            _tcs = new TaskCompletionSource<ArtifactDetailModalResult>();
            return await _tcs.Task;
        }

        private async Task UpdateArtifactSizesAsync()
        {
            try
            {
                var requireToCalculateSizeArtifacts = _artifacts.Where(c => c.ArtifactType != FsArtifactType.File);

                foreach (var artifact in requireToCalculateSizeArtifacts)
                {
                    await UpdateArtifactSizeAsync(artifact);
                    _artifactsSize = FsArtifactUtils.CalculateSizeStr(_artifacts.Sum(a => a.Size ?? 0));
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception);
            }
            
        }

        void SetCurrentArtifact(int index)
        {
            _currentArtifactForShowNumber = index;
            _currentArtifact = _artifacts[index];
        }

        async Task UpdateArtifactSizeAsync(FsArtifact artifact)
        {
            var path = artifact.FullPath;
            int lastUpdateSecond = 0;
            var sw = Stopwatch.StartNew();
            
            await foreach (var newSize in CalculateFolderOrDriveSize(path))
            {
                artifact.Size = newSize;
                var second = (int) sw.Elapsed.TotalSeconds;
                if (second != lastUpdateSecond)
                {
                    lastUpdateSecond = second;
                    await InvokeAsync(StateHasChanged);
                }
            }
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
