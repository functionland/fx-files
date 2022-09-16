using Functionland.FxFiles.App.Components.Common;

namespace Functionland.FxFiles.App.Pages
{
    public partial class MyDevice
    {

        public List<FileCardConfig> PinnedCards = new List<FileCardConfig>
        {
            new FileCardConfig(true, true, true, "Cs intenrship", ".txt", "date", "file size")
        };
    }
}
