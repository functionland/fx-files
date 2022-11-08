namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxBottomSheet
    {
        [Parameter, EditorRequired]
        public bool ShowCloseButton { get; set; } = false;

        [Parameter]
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

        [Parameter]
        public bool CanClose { get; set; } = true;


        public async Task Close()
        {
            if (CanClose)
            {
                IsOpen = false;
                await IsOpenChanged.InvokeAsync(IsOpen);
                await OnClose.InvokeAsync();
            }
            else
            {
                return;
            }
        }
    }
}
