namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxSearchInput
    {
        [Parameter, EditorRequired]
        public bool IsPartial { get; set; }

        [Parameter]
        public string? Text { get; set; }
    }
}
