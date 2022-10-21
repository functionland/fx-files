namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileCard
    {
        [Parameter]
        public FsArtifactType ArtifactType { get; set; }
        
        [Parameter]
        public FileCategoryType FileType { get; set; }

        [Parameter]
        public bool IsPinned { get; set; }

        [Parameter]
        public bool IsEnable { get; set; }

        [Parameter]
        public string? TagTitle { get; set; }

        [Parameter]
        public string? FileName { get; set; }

        [Parameter]
        public string? FileFormat { get; set; }

        [Parameter]
        public string? ModifiedDate { get; set; }

        [Parameter]
        public string? FileSize { get; set; }

        private string? _path;

        [Parameter]
        public string? FileImage { get; set; }
        

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
    }
}
