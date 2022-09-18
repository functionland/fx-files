namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxBottomSheet
    {
        [Parameter, EditorRequired]
        public bool IsCloseAble { get; set; } = false;

        [Parameter, EditorRequired]
        public RenderFragment? ContentRenderFragment { get; set; }

        [Parameter]
        public bool IsClose { get; set; } = true;

        [Parameter]
        public EventCallback<bool> IsCloseChanged { get; set; }

        public async Task Close()
        {
            ChangeCloseState();
            await IsCloseChanged.InvokeAsync(IsClose);
        }

        public void ChangeCloseState()
        {
            IsClose = !IsClose;
        }
    }
}
