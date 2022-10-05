namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileCard
    {
        [Parameter]
        public bool IsFile { get; set; } //TODO: remove this parameter after get size for folder pinned
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
        public string? FileImage
        {
            get => getFileImage(_path);
            set => _path = value;
        }

        private string getFileImage(string? path)
        {
            if(path != null)
            {
                var resultPath = "_content/Functionland.FxFiles.Client.Shared/" + path + ".HandleByApp=true";
                return resultPath;
            }

            return "_content/Functionland.FxFiles.Client.Shared/images/backgrounds/card-media.png";
        }
    }
}
