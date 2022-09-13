namespace Functionland.FxFiles.App.Pages
{
    public partial class ThemeSettings
    {
        [AutoInject]
        private ThemeInterop ThemeInterop = default!;

        public bool _isLoaded = false;
        private bool IsDarkMode { get; set; }
        private bool IsSystemTheme { get; set; }

        private FxTheme SystemTheme { get; set; }
        private FxTheme DesiredTheme { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isLoaded = true;

                DesiredTheme = await ThemeInterop.GetThemeAsync();
                SystemTheme = await ThemeInterop.GetSystemThemeAsync();

                IsSystemTheme = SystemTheme == DesiredTheme;
                IsDarkMode = DesiredTheme is FxTheme.Dark;

                await OnThemeChangedAsync(IsDarkMode);
                await OnUseSystemThemeAsync(IsSystemTheme);
                await ThemeInterop.RegisterForSystemThemeChangedAsync();

                StateHasChanged();
            }
        }

        private async Task OnUseSystemThemeAsync(bool isSystemTheme)
        {
            IsSystemTheme = isSystemTheme;
            await ThemeInterop.SetThemeAsync(IsSystemTheme ? SystemTheme : DesiredTheme);
            IsDarkMode = DesiredTheme is FxTheme.Dark;
            StateHasChanged();
        }

        private async Task OnThemeChangedAsync(bool isTheme)
        {
            IsDarkMode = isTheme;
            await ThemeInterop.SetThemeAsync(IsDarkMode ? FxTheme.Dark : FxTheme.Light);
            StateHasChanged();
        }
    }
}