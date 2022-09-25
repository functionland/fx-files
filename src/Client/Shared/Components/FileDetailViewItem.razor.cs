namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileDetailViewItem
    {
        [Parameter, EditorRequired]
        public string? FileDetailFieldName { get; set; }

        [Parameter, EditorRequired]
        public string? FileDetailFieldValue { get; set; }
    }
}
