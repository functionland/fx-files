namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxLink
    {
        [Parameter]
        public FxLinkSize FxLinkSize { get; set; }
        [Parameter]
        public bool IsEnabel { get; set; } = true;

        [Parameter, EditorRequired]
        public FxLinkIconSide FxLinkIconSide { get; set; }

        [Parameter, EditorRequired]
        public string? Link { get; set; }

        [Parameter, EditorRequired]
        public string? LinkText { get; set; }

    }

    public enum FxLinkIconSide
    {
        Left,
        Right,
        Regular
    }

    public enum FxLinkSize
    {
        Large
    }
}
