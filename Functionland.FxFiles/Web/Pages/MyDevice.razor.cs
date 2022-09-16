using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {

        public List<FileCardConfig> PinnedCards = new List<FileCardConfig>
        {
            new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size"),
            new FileCardConfig(true, true, true, "Fx Land", ".mp3", "date", "file size"),
            new FileCardConfig(true, true, true, "doument", ".pdf", "date", "file size"),
            new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size")
        };
    }
}
