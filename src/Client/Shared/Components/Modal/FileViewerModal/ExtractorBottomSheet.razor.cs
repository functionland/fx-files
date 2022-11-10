using System.Text;

using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Components.Modal
{
    public partial class ExtractorBottomSheet
    {
        [Parameter] public IFileService FileService { get; set; } = default!;
        [AutoInject] private IZipService ZipService { get; set; } = default!;

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

        private TaskCompletionSource<ExtractorBottomSheetResult>? _tcs;

        protected override Task OnInitAsync()
        {
            GoBackService.OnInit((Task () =>
            {
                HandleBackAsync();
                return Task.CompletedTask;
            }), true, false);
            return base.OnInitAsync();
        }

        public async Task<ExtractorBottomSheetResult> ExtractZipAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName, string? password = null, List<FsArtifact>? innerArtifacts = null)
        {
            var result = new ExtractorBottomSheetResult();

            try
            {
                var duplicateCount = 0;
                try
                {
                    duplicateCount = await ZipService.ExtractZippedArtifactAsync(
                        zipFilePath,
                        destinationFolderPath,
                        destinationFolderName,
                        innerArtifacts,
                        false,
                        password,
                        OnProgress,
                        _progressBarCts?.Token);
                }
                catch (InvalidPasswordException)
                {
                    if (_extractorPasswordModalRef == null)
                    {
                        result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                        _tcs?.SetResult(result);
                        return result;
                    }

                    var extractPasswordModalTitle = Localizer.GetString(AppStrings.ExtractPasswordModalTitle);
                    var extractPasswordModalLabel = Localizer.GetString(AppStrings.Password);
                    var extractBtnTitle = Localizer.GetString(AppStrings.Extract);
                    var passwordResult = await _extractorPasswordModalRef.ShowAsync(extractPasswordModalTitle,
                        string.Empty, string.Empty, string.Empty, extractBtnTitle, extractPasswordModalLabel);
                    if (passwordResult?.ResultType == InputModalResultType.Cancel)
                    {
                        result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                        _tcs?.SetResult(result);
                        return result;
                    }

                    try
                    {
                        duplicateCount = await ZipService.ExtractZippedArtifactAsync(
                            zipFilePath,
                            destinationFolderPath,
                            destinationFolderName,
                            innerArtifacts,
                            false,
                            passwordResult?.Result ??
                            throw new InvalidOperationException(
                                Localizer.GetString(nameof(AppStrings.PasswordEmptyMessage))),
                            OnProgress,
                            _progressBarCts?.Token);
                    }
                    catch (InvalidPasswordException)
                    {
                        FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorMessage)), Localizer.GetString(nameof(AppStrings.PasswordDidNotMatchedException)), FxToastType.Error);
                        result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                        _tcs?.SetResult(result);
                        return result;
                    }
                }
                catch (Exception)
                {
                    throw new DomainLogicException(Localizer.GetString(nameof(AppStrings.TheOpreationFailedMessage)));
                }

                if (_progressModalRef is null)
                {
                    result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                    _tcs?.SetResult(result);
                    ProgressBarOnCancel();
                    return result;
                }

                await _progressModalRef.ShowAsync(ProgressMode.Progressive,
                    Localizer.GetString(AppStrings.ExtractingFolder), true);
                _progressBarCts = new CancellationTokenSource();

                async Task OnProgress(ProgressInfo progressInfo)
                {
                    ProgressBarCurrentText = progressInfo.CurrentText ?? string.Empty;
                    ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? string.Empty;
                    ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
                    ProgressBarMax = progressInfo.MaxValue ?? 1;
                    await InvokeAsync(StateHasChanged);
                }

                await _progressModalRef.CloseAsync();

                if (duplicateCount <= 0)
                {
                    result.ExtractorResult = ExtractorBottomSheetResultType.Success;
                    _tcs = new TaskCompletionSource<ExtractorBottomSheetResult>();
                    _tcs.SetResult(result);
                    return await _tcs.Task;
                }

                if (_extractorConfirmationReplaceOrSkipModalRef == null)
                {
                    result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                    _tcs?.SetResult(result);
                    ProgressBarOnCancel();
                    return result;
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

                    await _progressModalRef.ShowAsync(ProgressMode.Progressive,
                        Localizer.GetString(AppStrings.ReplacingFiles), true);

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

                result.ExtractorResult = ExtractorBottomSheetResultType.Success;
                _tcs = new TaskCompletionSource<ExtractorBottomSheetResult>();
                _tcs.SetResult(result);
                return await _tcs.Task;
            }
            catch (Exception)
            {
                result.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                _tcs?.SetResult(result);
                throw;
            }
            finally
            {
                if (_progressModalRef != null)
                {
                    await _progressModalRef.CloseAsync();
                }

                _tcs = null;
            }
        }

        private static List<FsArtifact> GetShouldOverwriteArtifacts(List<FsArtifact> artifacts, List<FsArtifact> existArtifacts)
        {
            var pathExistArtifacts = existArtifacts.Select(a => a.FullPath);

            return artifacts.Where(artifact => pathExistArtifacts.Any(p => p.Equals(artifact.FullPath))).ToList();
        }

        private void HandleBackAsync()
        {
            var result = new ExtractorBottomSheetResult
            {
                ExtractorResult = ExtractorBottomSheetResultType.Cancel
            };
            _tcs?.SetResult(result);
            _tcs = null;
            _extractorPasswordModalRef?.Dispose();
        }
    }
}
