using System.Diagnostics.Metrics;

namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class SettingsPage
{
    [AutoInject] private ThemeInterop ThemeInterop = default!;
    [AutoInject] private InMemoryAppStateStore AppState { get; set; } = default!;

    private FxTheme DesiredTheme { get; set; }
    private string? CurrentTheme { get; set; }
    private string? CurrentVersion { get; set; }

    private int _counter = 0;
    private const int MaxCount = 7;

    protected override async Task OnInitAsync()
    {
        GoBackService.OnInit(null, true, true);

        DesiredTheme = await ThemeInterop.GetThemeAsync();

        if (DesiredTheme == FxTheme.Dark)
            CurrentTheme = Localizer.GetString(nameof(AppStrings.Night));
        else if (DesiredTheme == FxTheme.Light)
            CurrentTheme = Localizer.GetString(nameof(AppStrings.Day));
        else
            CurrentTheme = Localizer.GetString(nameof(AppStrings.System));

        GetAppVersion();
    }

    public void Login()
    {
        FxToast.Show(Localizer[nameof(AppStrings.ComingSoon)], Localizer[nameof(AppStrings.FutureFeature)], FxToastType.Info);
    }

    public void HandleTitleClick()
    {
        if (_counter >= MaxCount && AppState.IsAvailableForTest) return;

        _counter++;

        if(_counter >= MaxCount)
        {
            AppState.SetAvailableForTest(true);
        }
    }
    private void GetAppVersion()
    {
#if BlazorHybrid
        CurrentVersion = AppInfo.Current.VersionString;
#endif
    }
}