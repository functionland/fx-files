using System.Text;

using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class Extractor
    {
        [Parameter] public EventCallback<FileViewerResultType> ExtractResultCallback { get; set; }

        // Service
        [AutoInject] private ZipService ZipService { get; set; } = default!;
        [AutoInject] private IFileService FileService { get; set; } = default!;

        // Modals
        private InputModal? _extractorPasswordModalRef;
        private ProgressModal? _progressModalRef;
        private ConfirmationReplaceOrSkipModal? _extractorConfirmationReplaceOrSkipModalRef;

        // ProgressBar
        private string ProgressBarCurrentText { get; set; } = default!;
        private string ProgressBarCurrentSubText { get; set; } = default!;
        private int ProgressBarCurrentValue { get; set; }
        private int ProgressBarMax { get; set; }
        private CancellationTokenSource? _progressBarCts;
        private void ProgressBarOnCancel()
        {
            _progressBarCts?.Cancel();
        }

        private FileViewerResultType FileViewerResult { get; set; }

        protected override Task OnInitAsync()
        {
            GoBackService.OnInit((Task () =>
            {
                HandleBackAsync();
                return Task.CompletedTask;
            }), true, false);
            return base.OnInitAsync();
        }

        public async Task ExtractZipAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName, string? password = null, List<FsArtifact>? innerArtifacts = null)
        {
            if (_progressModalRef is null)
            {
                FileViewerResult = FileViewerResultType.Cancel;
                await ExtractResultCallback.InvokeAsync(FileViewerResult);
                return;
            }

            try
            {
                await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ExtractingFolder), true);
                _progressBarCts = new CancellationTokenSource();

                async Task OnProgress(ProgressInfo progressInfo)
                {
                    ProgressBarCurrentText = progressInfo.CurrentText ?? string.Empty;
                    ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? string.Empty;
                    ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                    ProgressBarMax = progressInfo.MaxValue ?? 1;
                    await InvokeAsync(StateHasChanged);
                }

                var duplicateCount = await ZipService.ExtractZippedArtifactAsync(
                    zipFilePath,
                    destinationFolderPath,
                    destinationFolderName,
                    innerArtifacts,
                     false,
                     password,
                     OnProgress,
                     _progressBarCts.Token);

                await _progressModalRef.CloseAsync();

                if (duplicateCount <= 0)
                {
                    FileViewerResult = FileViewerResultType.Cancel;
                    await ExtractResultCallback.InvokeAsync(FileViewerResult);
                    return;
                }

                if (_extractorConfirmationReplaceOrSkipModalRef == null)
                {
                    FileViewerResult = FileViewerResultType.Cancel;
                    await ExtractResultCallback.InvokeAsync(FileViewerResult);
                    return;
                }

                var existedArtifacts = await FileService.GetArtifactsAsync(destinationFolderPath).ToListAsync();
                List<FsArtifact> overwriteArtifacts = new();
                if (innerArtifacts != null)
                {
                    overwriteArtifacts = GetShouldOverwriteArtifacts(innerArtifacts, existedArtifacts);
                }

                var replaceResult = await _extractorConfirmationReplaceOrSkipModalRef.ShowAsync(duplicateCount);

                if (replaceResult?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                {

                    await _progressModalRef.ShowAsync(ProgressMode.Progressive, Localizer.GetString(AppStrings.ReplacingFiles), true);

                    _progressBarCts = new CancellationTokenSource();
                    await ZipService.ExtractZippedArtifactAsync(
                        zipFilePath,
                        destinationFolderPath,
                        destinationFolderName,
                        overwriteArtifacts,
                         true,
                         password,
                         OnProgress,
                         _progressBarCts.Token);
                }
                FileViewerResult = FileViewerResultType.Success;
            }
            finally
            {
                await _progressModalRef.CloseAsync();
            }
        }

        private static List<FsArtifact> GetShouldOverwriteArtifacts(List<FsArtifact> artifacts, List<FsArtifact> existArtifacts)
        {
            List<FsArtifact> overwriteArtifacts = new();
            var pathExistArtifacts = existArtifacts.Select(a => a.FullPath);
            foreach (var artifact in artifacts)
            {
                if (pathExistArtifacts.Any(p => p.StartsWith(artifact.FullPath)))
                {
                    overwriteArtifacts.Add(artifact);
                }
            }

            return overwriteArtifacts;
        }

        private void HandleBackAsync()
        {
            FileViewerResult = FileViewerResultType.Cancel;
            _extractorPasswordModalRef?.Dispose();
        }
    }
}
