namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxToast
    {
        [Parameter, EditorRequired]
        public FxToastType ToastType { get; set; }

        [Parameter, EditorRequired]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public string? Description { get; set; }

        [Parameter]
        public bool IsOpen { get; set; } = false;

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        private void Close()
        {
            IsOpen = false;
            IsOpenChanged.InvokeAsync(IsOpen);
        }
    }

    public enum FxToastType
    {
        Success,
        Warning,
        Info,
        Error
    }
}
