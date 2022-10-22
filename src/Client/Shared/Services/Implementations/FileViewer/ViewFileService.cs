namespace Functionland.FxFiles.Client.Shared.Services.Implementations.FileViewer
{
    public partial class ViewFileService<TFileService> : IViewFileService<TFileService>
        where TFileService : IFileService
    {
        [AutoInject] IEnumerable<IFileViewer> FileViewers { get; set; }

        [AutoInject] TFileService FileService { get; set; }
        [AutoInject] IExceptionHandler ExceptionHandler { get; set; }
        [AutoInject] IStringLocalizer<AppStrings> StringLocalizer { get; set; }
        public async Task ViewFile(FsArtifact artifact, string returnUrl)
        {
            var fileViewer = FileViewers.FirstOrDefault(fv => fv.IsExtenstionSupported(artifact.FullPath, FileService));

            if (fileViewer is null)
            {
#if BlazorHybrid
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(artifact.FullPath)
                    });
                }
                catch (UnauthorizedAccessException)
                {
                    ExceptionHandler?.Handle(new DomainLogicException(StringLocalizer.GetString(nameof(AppStrings.ArtifactUnauthorizedAccessException))));
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Handle(exception);
                }
#endif
                return;
            }

            await fileViewer.ViewAsync(artifact.FullPath, FileService, returnUrl);
        }
    }
}
