namespace Functionland.FxFiles.App.Components.Common
{
    public class ListItemConfig
    {
        public ListItemConfig(string? title, string? subTitle, bool? isPinned, string? iconStr, bool? isSelected)
        {
            Title = title;
            SubTitle = subTitle;
            IsPinned = isPinned;
            IconStr = iconStr;
            IsSelected = isSelected;
        }

        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public bool? IsPinned { get; set; }
        public string? IconStr { get; set; }
        public bool? IsSelected { get; set; }
    }
}
