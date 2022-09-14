using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        public ViewMode ViewMode = ViewMode.grid;
        public List<ListItemConfig> ListItems = new List<ListItemConfig>()
        {
            new ListItemConfig(ViewMode.grid, "CsInternship", "subtext1", true, "folder"),
            new ListItemConfig(ViewMode.grid, "text.txt", "subtext2", true, "text-file"),
            new ListItemConfig(ViewMode.grid, "green.mp3", "subtext3", true, "audio-file"),
            new ListItemConfig(ViewMode.grid, "word.pdf", "subtext4", true, "pdf-file"),
            new ListItemConfig(ViewMode.grid, "jojo.jpg", "subtext5", true, "photo-file")
        };
    }
}
