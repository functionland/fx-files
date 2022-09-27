namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileCard
    {
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

        [Parameter]
        public string? FileImage { get; set; } 
    }
}
