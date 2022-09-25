namespace Functionland.FxFiles.Client.Shared.Components
{
    public sealed class ThemeInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public Func<FxTheme, Task>? SystemThemeChanged;

        public ThemeInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/Functionland.FxFiles.Client.Shared/scripts/themeJsInterop.js").AsTask());
        }

        public ValueTask<FxTheme> GetThemeAsync() =>
            GetThemeByIdentifierAsync("getTheme");

        public ValueTask<FxTheme> GetSystemThemeAsync() =>
            GetThemeByIdentifierAsync("getSystemTheme");

        public async ValueTask RegisterForSystemThemeChangedAsync()
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync(
                "registerForSystemThemeChanged",
                DotNetObjectReference.Create(this),
                nameof(OnSystemThemeChanged));
        }

        [JSInvokable]
        public Task OnSystemThemeChanged(bool isDarkTheme) =>
            SystemThemeChanged?.Invoke(
                isDarkTheme ? FxTheme.Dark : FxTheme.Light)
            ?? Task.CompletedTask;

        private async ValueTask<FxTheme> GetThemeByIdentifierAsync(string identifier)
        {
            var module = await _moduleTask.Value;
            var theme = await module.InvokeAsync<string>(identifier);
            return theme != null ? (FxTheme)Enum.Parse(typeof(FxTheme), theme) : FxTheme.Light;
        }

        public async Task SetThemeAsync(FxTheme theme)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setTheme", theme.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }

    public enum FxTheme
    {
        Dark,
        Light,
        System
    }
}
