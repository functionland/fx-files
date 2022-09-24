namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxTextInput
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public string? Margin { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public string? FieldQuestion { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; }
    }
}