using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Components
{
    public partial class FileDetailViewBottomActionItem
    {
        [Parameter, EditorRequired]
        public ActionIcon ActionIcon { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
    }

    public enum ActionIcon
    {
        Download,
        Upload,
        Move,
        Pin,
        More
    }
}
