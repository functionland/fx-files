using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Components
{
    public partial class ListItem
    {
        [Parameter, EditorRequired]
        public ListItemConfig Config { get; set; } = new ListItemConfig(ViewMode.list, "", "", false, "");
    }
}
