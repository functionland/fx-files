using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.App.Shared;

public partial class MainLayout
{
    [AutoInject]
    private ThemeInterop ThemeInterop = default!;

    private bool IsSystemTheme;
    private bool IsDarkMode;

    private FxTheme DesiredTheme;
    private FxTheme SystemTheme;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        DesiredTheme = await ThemeInterop.GetThemeAsync();
        SystemTheme = await ThemeInterop.GetSystemThemeAsync();
        IsDarkMode = DesiredTheme is FxTheme.Dark;
        await OnThemeChanged(IsDarkMode);
        await OnUseSystemTheme(IsDarkMode);
        await ThemeInterop.RegisterForSystemThemeChangedAsync();
        StateHasChanged();
    }

    private async Task OnUseSystemTheme(bool value)
    {
        IsSystemTheme = value;
        await ThemeInterop.SetThemeAsync(IsSystemTheme ? SystemTheme : DesiredTheme);
    }

    private async Task OnThemeChanged(bool value)
    {
        IsDarkMode = value;
        await ThemeInterop.SetThemeAsync(IsDarkMode ? FxTheme.Dark : FxTheme.Light);
    }
}