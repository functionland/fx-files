using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Components
{
    public partial class ListItem
    {
        [Parameter, EditorRequired]
        public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.list;

        [Parameter, EditorRequired]
        public ListItemConfig ItemConfig { get; set; } = new ListItemConfig("", "", false, "");
    }
}
