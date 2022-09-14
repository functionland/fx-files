namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxButton
    {
        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public string? MetaDataText { get; set; }

        [Parameter]
        public string? Link { get; set; }

        [Parameter]
        public string? Width { get; set; }

        [Parameter]
        public string? Height { get; set; }

        [Parameter]
        public string? TextColor { get; set; }

        [Parameter]
        public string? BorderWidth { get; set; }

        [Parameter]
        public string? BorderColor { get; set; }

        [Parameter]
        public string? Margin { get; set; }

        [Parameter]
        public string? Padding { get; set; }

        [Parameter]
        public string? LeftIcon { get; set; }

        [Parameter]
        public string? RightIcon { get; set; }

        [Parameter]
        public string? BorderRadius { get; set; }

        [Parameter]
        public string? BackgroundColor { get; set; }

        [Parameter]
        public string? VerticalAlignment { get; set; }

        [Parameter]
        public string? HorizontalAlignment { get; set; }

        [Parameter]
        public FxButtonTextAlignment? HorizontalTextAlignment { get; set; }

        [Parameter]
        public FxButtonSize? ButtonSize { get; set; }

        [Parameter, EditorRequired]
        public FxButtonStyle? ButtonStyle { get; set; }
    }

    public enum FxButtonSize
    {
        Small,
        Large,
        Stretch,
    }

    public enum FxButtonStyle
    {
        Normal,
        Outline,
        Disabled,
        MenuLine,
    }

    public enum FxButtonTextAlignment
    {
        End,
        Start,
        Center,
    }
}