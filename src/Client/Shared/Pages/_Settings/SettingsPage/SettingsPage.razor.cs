using System.Diagnostics;
using System.Reflection;

namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class SettingsPage
    {
        [AutoInject]
        private ThemeInterop ThemeInterop = default!;

        private FxTheme DesiredTheme { get; set; }
        private string? CurrentTheme { get; set; }

        private string? CurrentVersion { get; set; }

        protected override async Task OnInitAsync()
        {
            DesiredTheme = await ThemeInterop.GetThemeAsync();

            if (DesiredTheme == FxTheme.Dark)
                CurrentTheme = Localizer.GetString(nameof(AppStrings.Night));
            else if (DesiredTheme == FxTheme.Light)
                CurrentTheme = Localizer.GetString(nameof(AppStrings.Day));
            else
                CurrentTheme = Localizer.GetString(nameof(AppStrings.System));

            GetAppVersion();
        }

        private void GetAppVersion()
        {
#if BlazorHybrid
            CurrentVersion = AppInfo.Current.VersionString;
#endif
        }
    }
}