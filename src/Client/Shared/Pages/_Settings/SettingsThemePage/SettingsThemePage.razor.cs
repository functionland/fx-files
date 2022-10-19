namespace Functionland.FxFiles.Client.Shared.Pages
{
    public partial class SettingsThemePage
    {
        [AutoInject]
        private ThemeInterop ThemeInterop = default!;

        public bool _isLoaded = false;
        private bool IsDarkMode { get; set; }
        private bool IsSystemTheme { get; set; }

        private FxTheme SystemTheme { get; set; }
        private FxTheme CurrentTheme { get; set; }

        protected override Task OnInitAsync()
        {
            GoBackService.OnInit((Task () =>
            {
                HandleToolbarBack();
                StateHasChanged();
                return Task.CompletedTask;
            }), true, false);

            return base.OnInitAsync();
        }

        private bool _isSystemThemeDark;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isSystemThemeDark = await ThemeInterop.GetSystemThemeAsync() is FxTheme.Dark;
                _isLoaded = true;
                ThemeInterop.SystemThemeChanged = async (FxTheme theme) =>
                {
                    _isSystemThemeDark = theme is FxTheme.Dark;
                    StateHasChanged();
                };

                CurrentTheme = await ThemeInterop.GetThemeAsync();

                //SystemTheme = await ThemeInterop.GetSystemThemeAsync();
                //IsSystemTheme = SystemTheme == DesiredTheme;
                //IsDarkMode = DesiredTheme is FxTheme.Dark;
                //await ChangeThemeAsync();
                //await OnUseSystemThemeAsync(IsSystemTheme);

                await ThemeInterop.RegisterForSystemThemeChangedAsync();

                StateHasChanged();
            }
        }

        //private async Task OnUseSystemThemeAsync(bool isSystemTheme)
        //{
        //    IsSystemTheme = isSystemTheme;
        //    await ThemeInterop.SetThemeAsync(IsSystemTheme ? SystemTheme : DesiredTheme);
        //    IsDarkMode = DesiredTheme is FxTheme.Dark;
        //    StateHasChanged();
        //}

        private async Task ChangeThemeAsync()
        {
            await ThemeInterop.SetThemeAsync(CurrentTheme);
        }

        private void HandleToolbarBack()
        {
            NavigationManager.NavigateTo("settings", false, true);
        }
    }
}