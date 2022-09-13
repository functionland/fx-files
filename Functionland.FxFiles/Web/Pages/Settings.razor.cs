namespace Functionland.FxFiles.App.Pages
{
    public partial class Settings
    {
        [AutoInject]
        private ThemeInterop ThemeInterop = default!;

        private FxTheme DesiredTheme { get; set; }
        private string CurrentTheme { get; set; }

        protected override async Task OnInitAsync()
        {
            DesiredTheme = await ThemeInterop.GetThemeAsync();

            if (DesiredTheme == FxTheme.Dark)
                CurrentTheme = Localizer.GetString(nameof(AppStrings.Night));
            else if (DesiredTheme == FxTheme.Light)
                CurrentTheme = Localizer.GetString(nameof(AppStrings.Day));
        }
    }
}