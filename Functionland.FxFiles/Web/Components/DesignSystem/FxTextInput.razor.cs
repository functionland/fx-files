namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxTextInput
    {
        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public EventCallback<string?> TextChanged { get; set; }

        private string? _text { get; set; }
        [Parameter]
        public string? Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;

                _text = value;
                TextChanged.InvokeAsync(value);
            }
        }

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