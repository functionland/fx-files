namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxTag
    {
        [Parameter]
        public FxTagType TagType { get; set; }

        [Parameter, EditorRequired]
        public string? Title { get; set; }
        
        [Parameter, EditorRequired]
        public string? Width { get; set; }

    }

    public enum FxTagType
    {
        Regular,
        Left,
        Right
    }
}
