using Functionland.FxFiles.App.Components.Common;
using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.App.Components
{
    public partial class ListItem
    {
        [Parameter, EditorRequired]
        public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.list;

        [Parameter]
        public EventCallback<PointerEventArgs> OnPointerDown { get; set; }

        [Parameter]
        public EventCallback<PointerEventArgs> OnPointerUp { get; set; }

        [Parameter]
        public bool IsSelectionMode { get; set; }

        [Parameter]
        public bool IsSelected { get; set; }

        [Parameter, EditorRequired]
        public ListItemConfig ItemConfig { get; set; } = new ListItemConfig("", "", false, "", false);
    }
}
