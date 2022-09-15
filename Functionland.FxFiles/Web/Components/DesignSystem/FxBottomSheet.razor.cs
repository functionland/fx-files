namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxBottomSheet
    {
        [Parameter, EditorRequired]
        public bool IsCloseAble { get; set; } = false;

        [Parameter, EditorRequired]
        public RenderFragment? ContentRenderFragment { get; set; }
    }
}
