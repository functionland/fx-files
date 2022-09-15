namespace Functionland.FxFiles.App.Components.DesignSystem
{
    public partial class FxToast
    {
        [Parameter, EditorRequired]
        public FxToastType ToastType { get; set; }

        [Parameter, EditorRequired]
        public string? Title { get; set; }

        [Parameter, EditorRequired]
        public string? Description { get; set; }
    }

    public enum FxToastType
    {
        Success,
        Warning,
        Info,
        Error
    }
}
