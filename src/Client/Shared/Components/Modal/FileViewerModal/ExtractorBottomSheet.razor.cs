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
        private double ProgressBarCurrentValue { get; set; }
        private int ProgressBarMax { get; set; }
        private CancellationTokenSource? _progressBarCts;
        private void ProgressBarOnCancel()
        {
            _progressBarCts?.Cancel();
        }

        private TaskCompletionSource<ExtractorBottomSheetResult>? _tcs;

        private string? _password;
        
        private ExtractorBottomSheetResult ExtractorBottomSheetResult { get; set; } = new();

        public async Task<ExtractorBottomSheetResult> ShowAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName, List<FsArtifact>? innerArtifacts = null)
        {
            GoBackService.SetState((Task () =>
            {
                HandleBackAsync();
                return Task.CompletedTask;
            }), true, false);
            _tcs = new TaskCompletionSource<ExtractorBottomSheetResult>();

            await HandleExtractZipAsync(zipFilePath, destinationFolderPath, destinationFolderName, innerArtifacts);

            var result = await _tcs.Task;

            GoBackService.ResetPreviousState();

            return result;
        }

        private async Task HandleExtractZipAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName, List<FsArtifact>? innerArtifacts = null)
        {

            if (_extractorPasswordModalRef == null || _progressModalRef is null || _extractorConfirmationReplaceOrSkipModalRef == null)
            {
                ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                _tcs?.SetResult(ExtractorBottomSheetResult);
                return;
            }
            _progressBarCts = new CancellationTokenSource();

            try
            {
                int? duplicateCount;
                try
                {
                    await ShowProgressModal();
                    duplicateCount = await ExtractZipAsync(zipFilePath, destinationFolderPath, destinationFolderName,
                        _password, innerArtifacts);
                }
                catch (PasswordDidNotMatchedException)
                {
                    FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorTitle)),
                        Localizer.GetString(nameof(AppStrings.PasswordDidNotMatchedException)), FxToastType.Error);
                    ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                    _tcs?.SetResult(ExtractorBottomSheetResult);
                    return;
                }
                catch (NotSupportedEncryptedFileException exception)
                {
                    FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorTitle)),
                        exception.Message, FxToastType.Error);
                    ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                    _tcs?.SetResult(ExtractorBottomSheetResult);
                    return;
                }
                catch (Exception)
                {
                    throw new DomainLogicException(Localizer.GetString(nameof(AppStrings.TheOpreationFailedMessage)));
                }
                finally
                {
                    await _progressModalRef.CloseAsync();
                    ProgressBarOnCancel();
                    _password = null;
                }

                switch (duplicateCount)
                {
                    case null:
                        return;
                    case <= 0:
                        ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Success;
                        _tcs?.SetResult(ExtractorBottomSheetResult);
                        return;
                }

                var overwriteArtifacts = await GetOverwritableArtifacts(zipFilePath, Path.Combine(destinationFolderPath, destinationFolderName), innerArtifacts);

                var replaceResult = await _extractorConfirmationReplaceOrSkipModalRef.ShowAsync(duplicateCount ?? throw new InvalidOperationException(Localizer.GetString(AppStrings.TheOpreationFailedMessage)));

                if (replaceResult?.ResultType == ConfirmationReplaceOrSkipModalResultType.Replace)
                {
                    _progressBarCts = new CancellationTokenSource();
                    await ShowProgressModal();
                    await ExtractZipAsync(zipFilePath, destinationFolderPath, destinationFolderName, _password, overwriteArtifacts, true);
                }

                ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Success;
                _tcs?.SetResult(ExtractorBottomSheetResult);
            }
            catch (Exception)
            {
                ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                _tcs?.SetResult(ExtractorBottomSheetResult);
                throw;
            }
            finally
            {
                if (_progressModalRef != null)
                {
                    await _progressModalRef.CloseAsync();
                }
                ProgressBarOnCancel();
                _password = null;
            }
        }

        private async Task<List<FsArtifact>> GetOverwritableArtifacts(string zipFilePath, string destinationFolderPath,
            List<FsArtifact>? innerArtifacts)
        {
            var existedArtifacts = await FileService.GetArtifactsAsync(destinationFolderPath).ToListAsync();
            var readInnerArtifacts = await ZipService.GetAllArtifactsAsync(zipFilePath, _password, _progressBarCts?.Token);
            var overwriteArtifacts = GetShouldOverwriteArtifacts(readInnerArtifacts, existedArtifacts);
            return overwriteArtifacts;
        }

        private async Task ShowProgressModal()
        {
            if (_progressModalRef != null)
            {
                await _progressModalRef.ShowAsync(ProgressMode.Progressive,
                    Localizer.GetString(AppStrings.ExtractingFolder), true);
            }
        }

        private async Task<int?> ExtractZipAsync(string zipFilePath, string destinationFolderPath, string destinationFolderName,
            string? password, List<FsArtifact>? innerArtifacts, bool overwrite = false)
        {
            var duplicateCount = 0;
            try
            {
                duplicateCount = await ZipService.ExtractZippedArtifactAsync(
                   zipFilePath,
                   destinationFolderPath,
                   destinationFolderName,
                   innerArtifacts,
                   overwrite,
                   password,
                   OnProgress,
                   _progressBarCts?.Token);
            }
            catch (InvalidPasswordException)
            {
                if (_progressModalRef != null)
                {
                    await _progressModalRef.CloseAsync();
                }

                ProgressBarOnCancel();
                await GetPasswordFromInputAsync();
                switch (_password)
                {
                    case "":
                        FxToast.Show(Localizer.GetString(nameof(AppStrings.ToastErrorTitle)),
                            Localizer.GetString(nameof(AppStrings.PasswordEmptyMessage)), FxToastType.Error);
                        ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                        _tcs = new TaskCompletionSource<ExtractorBottomSheetResult>();
                        _tcs?.SetResult(ExtractorBottomSheetResult);
                        return null;
                    case null:
                        return null;
                    default:
                        try
                        {
                            _progressBarCts = new CancellationTokenSource();
                            await ShowProgressModal();
                            duplicateCount = await ZipService.ExtractZippedArtifactAsync(zipFilePath, destinationFolderPath,
                                destinationFolderName, innerArtifacts, overwrite, _password, OnProgress, _progressBarCts?.Token);
                        }
                        catch (PasswordDidNotMatchedException)
                        {
                            throw;
                        }
                        catch (Exception)
                        {
                            throw new DomainLogicException(Localizer.GetString(nameof(AppStrings.TheOpreationFailedMessage)));
                        }
                        finally
                        {
                            ProgressBarOnCancel();
                            if (_progressModalRef != null)
                            {
                                await _progressModalRef.CloseAsync();
                            }

                            _password = null;
                        }

                        break;
                }
            }
            finally
            {
                ProgressBarOnCancel();
                if (_progressModalRef != null)
                {
                    await _progressModalRef.CloseAsync();
                }

                _password = null;
            }
            return duplicateCount;
        }

        private async Task GetPasswordFromInputAsync()
        {
            var extractPasswordModalTitle = Localizer.GetString(AppStrings.ExtractPasswordModalTitle);
            var extractPasswordModalLabel = Localizer.GetString(AppStrings.Password);
            var extractBtnTitle = Localizer.GetString(AppStrings.Extract);
            if (_extractorPasswordModalRef != null)
            {
                var passwordResult = await _extractorPasswordModalRef.ShowAsync(extractPasswordModalTitle,
                    string.Empty, string.Empty, string.Empty, extractBtnTitle, extractPasswordModalLabel);
                _password = passwordResult.Result;
                if (passwordResult?.ResultType == InputModalResultType.Cancel || _password == null)
                {
                    ExtractorBottomSheetResult.ExtractorResult = ExtractorBottomSheetResultType.Cancel;
                    _tcs?.SetResult(ExtractorBottomSheetResult);
                }
            }
        }

        private async Task OnProgress(ProgressInfo progressInfo)
        {
            ProgressBarCurrentText = progressInfo.CurrentText ?? string.Empty;
            ProgressBarCurrentSubText = progressInfo.CurrentSubText ?? string.Empty;
            ProgressBarCurrentValue = progressInfo.CurrentValue ?? 0;
            ProgressBarMax = progressInfo.MaxValue ?? 1;
            await InvokeAsync(StateHasChanged);
        }

        private static List<FsArtifact> GetShouldOverwriteArtifacts(List<FsArtifact> artifacts, List<FsArtifact> existArtifacts)
        {
            var nameExistArtifacts = existArtifacts.Select(a => a.Name);

            return artifacts.Where(artifact => nameExistArtifacts.Any(p => p.Equals(artifact.Name))).ToList();
        }


        private void HandleBackAsync()
        {
            var result = new ExtractorBottomSheetResult
            {
                ExtractorResult = ExtractorBottomSheetResultType.Cancel
            };
            _tcs?.SetResult(result);
        }
    }
}
