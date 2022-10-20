namespace Functionland.FxFiles.Client.Shared.Components;

public sealed class ThemeInterop
{
    private readonly IJSRuntime _js;
    public Func<FxTheme, Task>? SystemThemeChanged;

    public ThemeInterop(IJSRuntime jsRuntime)
    {
        _js = jsRuntime;
    }

    public async Task<FxTheme> GetThemeAsync()
    {
        var theme = await _js.InvokeAsync<string>("FxTheme.getTheme");
        return theme != null ? (FxTheme)Enum.Parse(typeof(FxTheme), theme) : FxTheme.Light;
    }

    public async Task<FxTheme> GetSystemThemeAsync()
    {
        var theme = await _js.InvokeAsync<string>("FxTheme.getSystemTheme");
        return theme != null ? (FxTheme)Enum.Parse(typeof(FxTheme), theme) : FxTheme.Light;
    }

    public async ValueTask RegisterForSystemThemeChangedAsync()
    {
        await _js.InvokeVoidAsync(
            "FxTheme.registerForSystemThemeChanged",
            DotNetObjectReference.Create(this),
            nameof(OnSystemThemeChanged));
    }

    [JSInvokable]
    public Task OnSystemThemeChanged(bool isDarkTheme)
    {
        return SystemThemeChanged?.Invoke( isDarkTheme ? FxTheme.Dark : FxTheme.Light) ?? Task.CompletedTask;
    }

    public async Task SetThemeAsync(FxTheme theme)
    {
        await _js.InvokeVoidAsync("FxTheme.setTheme", theme.ToString());
    }

    public async Task ApplyThemeAsync(FxTheme theme)
    {
        await _js.InvokeVoidAsync("FxTheme.applyTheme", theme.ToString());
    }
}