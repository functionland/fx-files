using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FxLink
    {
        [Parameter]
        public FxLinkSize FxLinkSize { get; set; }
        [Parameter]
        public bool IsEnabel { get; set; } = true;

        [Parameter, EditorRequired]
        public FxLinkIconSide FxLinkIconSide { get; set; }

        [Parameter]
        public string? Link { get; set; }

        [Parameter, EditorRequired]
        public string? LinkText { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnHandelClick { get; set; } = default!;

    }

    public enum FxLinkIconSide
    {
        Left,
        Right,
        Regular
    }

    public enum FxLinkSize
    {
        Large
    }
}
