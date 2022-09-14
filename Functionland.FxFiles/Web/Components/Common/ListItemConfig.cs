namespace Functionland.FxFiles.App.Components.Common
{
    public class ListItemConfig
    {
        public ListItemConfig(ViewMode viewMode, string? title, string? subTitle, bool? isPinned, string? iconStr)
        {
            ViewMode = viewMode;
            Title = title;
            SubTitle = subTitle;
            IsPinned = isPinned;
            IconStr = iconStr;
        }

        public ViewMode ViewMode { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public bool? IsPinned { get; set; }
        public string? IconStr { get; set; }
    }

    public enum ViewMode
    {
        list,
        grid
    }
}
