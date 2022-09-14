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
        if (firstRender)
        {
            DesiredTheme = await ThemeInterop.GetThemeAsync();
            SystemTheme = await ThemeInterop.GetSystemThemeAsync();

            IsDarkMode = DesiredTheme is FxTheme.Dark;
            IsSystemTheme = DesiredTheme is FxTheme.System;

            if (IsSystemTheme)
                await ThemeInterop.SetThemeAsync(IsSystemTheme ? SystemTheme : DesiredTheme);
            else
                await ThemeInterop.SetThemeAsync(IsDarkMode ? FxTheme.Dark : FxTheme.Light);

            await ThemeInterop.RegisterForSystemThemeChangedAsync();
            StateHasChanged();
        }
    }
}