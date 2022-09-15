namespace Functionland.FxFiles.App.Components
{
    public partial class FileCard
    {
        [Parameter]
        public bool IsFavorite { get; set; }

        [Parameter]
        public bool IsDisable { get; set; }

        [Parameter, EditorRequired]
        public string? FileName { get; set; }

        [Parameter, EditorRequired]
        public string? FileFormat { get; set; }

        [Parameter, EditorRequired]
        public string? ModifiedDate { get; set; }
        
        [Parameter, EditorRequired]
        public string? FileSize { get; set; }
        
        public bool IsPressed { get; set; }
    }
}
