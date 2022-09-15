using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        public ViewMode ViewMode = ViewMode.list;
        public List<ListItemConfig> ListItems = new List<ListItemConfig>()
        {
            new ListItemConfig(ViewMode.list, "CsInternship", "subtext1", true, "folder"),
            new ListItemConfig(ViewMode.list, "text.txt", "subtext2", true, "text-file"),
            new ListItemConfig(ViewMode.list, "green.mp3", "subtext3", true, "audio-file"),
            new ListItemConfig(ViewMode.list, "word.pdf", "subtext4", true, "pdf-file"),
            new ListItemConfig(ViewMode.list, "jojo.jpg", "subtext5", true, "photo-file")
        };
    }
}
