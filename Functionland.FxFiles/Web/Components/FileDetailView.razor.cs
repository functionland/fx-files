namespace Functionland.FxFiles.App.Components
{
    public partial class FileDetailView
    {
        [Parameter, EditorRequired]
        public bool IsImageFile { get; set; } = false;

        [Parameter, EditorRequired]
        public string? FileName { get; set; }
    }
}
