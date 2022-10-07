using Microsoft.AspNetCore.Components.Web;

namespace Functionland.FxFiles.Client.Shared.Shared;

public partial class MainLayout
{
    [AutoInject]
    private ThemeInterop ThemeInterop = default!;

    private bool IsSystemTheme;
    private bool IsDarkMode;

    private FxTheme DesiredTheme;
    private FxTheme SystemTheme;
    private bool _isLoading = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _isLoading = true;
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
            _isLoading = false;
            StateHasChanged();
        }
        else
        {
            _isLoading = false;
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
