using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {
        public string ListTitle = "My Device";
        public List<ListItemConfig> ListItems = new List<ListItemConfig>()
        {
            new ListItemConfig("CsInternship", "subtext1", true, "folder"),
            new ListItemConfig("text.txt", "subtext2", true, "text-file"),
            new ListItemConfig("green.mp3", "subtext3", true, "audio-file"),
            new ListItemConfig("word.pdf", "subtext4", true, "pdf-file"),
            new ListItemConfig("jojo.jpg", "subtext5", true, "photo-file")
        };

    }
}
