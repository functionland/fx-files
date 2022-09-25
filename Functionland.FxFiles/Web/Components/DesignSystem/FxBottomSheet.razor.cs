namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxBottomSheet
    {
        [Parameter, EditorRequired]
        public bool ShowCloseButton { get; set; } = false;

        [Parameter, EditorRequired]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public bool IsFullScreenMode { get; set; } = false;

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public bool IsOpen { get; set; } = true;


        public async Task Close()
        {
            IsOpen = false;
            await IsOpenChanged.InvokeAsync(IsOpen);
            await OnClose.InvokeAsync();
        }
    }
}
