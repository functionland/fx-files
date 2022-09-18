namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxTextInput
    {
        [Parameter]
        public string? Text { get; set; }

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? PlaceHolder { get; set; }

        [Parameter]
        public string? QuestionText { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; }
    }
}