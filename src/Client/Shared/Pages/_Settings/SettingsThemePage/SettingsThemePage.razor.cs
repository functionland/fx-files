namespace Functionland.FxFiles.Client.Shared.Pages;

public partial class SettingsThemePage
{
    [AutoInject] private ThemeInterop ThemeInterop = default!;

    private FxTheme CurrentTheme { get; set; }

    protected override Task OnInitAsync()
    {
        GoBackService.SetState((Task () =>
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

            ThemeInterop.SystemThemeChanged = (FxTheme theme) =>
            {
                _isSystemThemeDark = theme is FxTheme.Dark;
                StateHasChanged();
                return Task.CompletedTask;
            };

            CurrentTheme = await ThemeInterop.GetThemeAsync();

            await ThemeInterop.RegisterForSystemThemeChangedAsync();

            StateHasChanged();
        }
    }

    private async Task ChangeThemeAsync()
    {
        await ThemeInterop.SetThemeAsync(CurrentTheme);
    }

    private void HandleToolbarBack()
    {
        NavigationManager.NavigateTo("settings", false, true);
    }
}