namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxDivider
    {
        [Parameter, EditorRequired]
        public FxDividerType FxDividerType { get; set; }

        [Parameter, EditorRequired]
        public FxDividerSize FxDividerSize { get; set; }

        [Parameter, EditorRequired]
        public FxDividerMode FxDividerMode { get; set; }
    }

    public enum FxDividerType
    {
        Horizontal,
        Vertical
    }

    public enum FxDividerSize
    {
        Thin,
        Regular,
        Bold
    }

    public enum FxDividerMode
    {
        Solid,
        Dotted
    }
}
