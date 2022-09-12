using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Functionland.FxFiles.App.Pages
{
    public partial class ThemeSettings
    {
        [AutoInject]
        private ThemeInterop ThemeInterop = default!;

        [AutoInject]
        private ProtectedLocalStorage _protectedLocalStorage { get; set; } = default!;

        private bool IsSystemTheme { get; set; }
        private bool IsDarkMode { get; set; }

        private FxTheme DesiredTheme { get; set; }
        private FxTheme SystemTheme { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                DesiredTheme = await ThemeInterop.GetThemeAsync();
                SystemTheme = await ThemeInterop.GetSystemThemeAsync();
                IsDarkMode = DesiredTheme is FxTheme.Dark;
                await OnThemeChangedAsync(IsDarkMode);
                IsSystemTheme = await IsSystemThemeActiveAsync();
                await OnUseSystemThemeAsync(IsSystemTheme);
                await ThemeInterop.RegisterForSystemThemeChangedAsync();
                StateHasChanged();
            }
        }

        private async Task OnUseSystemThemeAsync(bool isSystemTheme)
        {
            IsSystemTheme = isSystemTheme;

            await ThemeInterop.SetThemeAsync(IsSystemTheme ? SystemTheme : DesiredTheme);

            if (DesiredTheme is FxTheme.Dark)
            {
                IsDarkMode = true;
            }
            else
            {
                IsDarkMode = false;
            }
            await SetSystemThemeAsync(isSystemTheme);

            StateHasChanged();
        }

        private async Task OnThemeChangedAsync(bool isTheme)
        {
            IsDarkMode = isTheme;
            await ThemeInterop.SetThemeAsync(IsDarkMode ? FxTheme.Dark : FxTheme.Light);
            StateHasChanged();
        }

        public async Task<bool> IsSystemThemeActiveAsync()
        {
            var systemTheme = await _protectedLocalStorage.GetAsync<bool>("systemTheme");
            return systemTheme.Success ? systemTheme.Value : false;
        }

        public async Task SetSystemThemeAsync(bool isSystemTheme)
        {
            await _protectedLocalStorage.SetAsync("systemTheme", isSystemTheme);
        }
    }
}