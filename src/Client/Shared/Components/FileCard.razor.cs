namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileCard
    {
        [Parameter]
        public FsArtifact Artifact { get; set; } = default;

        [Parameter]
        public bool IsDisabled { get; set; } = false;

        [Parameter]
        public string? TagTitle { get; set; } = string.Empty;

        [Parameter]
        public IFileService? FileService { get; set; }

        [Parameter] 
        public IArtifactThumbnailService<IFileService> ThumbnailService { get; set; } = default!;

        public PathProtocol Protocol =>
            FileService switch
            {
                ILocalDeviceFileService => PathProtocol.ThumbnailStorageMedium,
                IFulaFileService => PathProtocol.ThumbnailStorageMedium,
                _ => throw new InvalidOperationException($"Unsupported file service: {FileService}")
            };

        protected override async Task OnInitAsync()
        { 
            await GetThumbnailAsync();
        }

        public string GetArtifactIcon(FsArtifactType artifactType, FileCategoryType fileType)
        {
            if (artifactType == FsArtifactType.File)
            {
                switch (fileType)
                {
                    case FileCategoryType.Document:
                        return "text-file-icon";
                    case FileCategoryType.Other:
                        return "text-file-icon";
                    case FileCategoryType.Pdf:
                        return "pdf-file-icon";
                    case FileCategoryType.Image:
                        return "photo-file-icon";
                    case FileCategoryType.Audio:
                        return "audio-file-icon";
                    case FileCategoryType.Video:
                        return "video-file-icon";
                    case FileCategoryType.App:
                        return "app-file-icon";
                }
            }

            return "folder-icon";
        }

        private async Task GetThumbnailAsync()
        {
            try
            {
                if (Artifact.ThumbnailPath is not null)
                    return;

                Artifact.ThumbnailPath = await ThumbnailService.GetOrCreateThumbnailAsync(Artifact, ThumbnailScale.Medium);
                string Test = "";

            }
            catch (Exception exception)
            {
                ExceptionHandler.Track(exception);
            }
        }


    }
}
