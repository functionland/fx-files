using Microsoft.AspNetCore.Components.Routing;

using System.Reflection;

namespace Functionland.FxFiles.Client.Shared;

public partial class App
{
#if BlazorWebAssembly && !BlazorHybrid
    private List<Assembly> _lazyLoadedAssemblies = new();
    [AutoInject] private Microsoft.AspNetCore.Components.WebAssembly.Services.LazyAssemblyLoader _assemblyLoader = default!;
#endif

    [AutoInject] private IJSRuntime _jsRuntime = default!;

    private bool _cultureHasNotBeenSet = true;
    private bool _themeHasNotBeenSet = true;


    [AutoInject] private ThemeInterop ThemeInterop = default!;

    private bool IsSystemTheme;
    private bool IsDarkMode;

    private FxTheme DesiredTheme;
    private FxTheme SystemTheme;

    

    private async Task OnNavigateAsync(NavigationContext args)
    {
        // Blazor Server & Pre Rendering use created cultures in UseRequestLocalization middleware
        // Android, windows and iOS have to set culture programmatically.
        // Browser is gets handled in Web project's Program.cs\
        if (_themeHasNotBeenSet)
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
#if BlazorHybrid && MultilingualEnabled
        if (_cultureHasNotBeenSet)
        {
            _cultureHasNotBeenSet = false;
            var preferredCultureCookie = Preferences.Get(".AspNetCore.Culture", null);
            CultureInfoManager.SetCurrentCulture(preferredCultureCookie);
        }
#endif

#if BlazorWebAssembly && !BlazorHybrid
        if (args.Path.Contains("some-lazy-loaded-page") && _lazyLoadedAssemblies.Any(asm => asm.GetName().Name == "SomeAssembly") is false)
        {
            var assemblies = await _assemblyLoader.LoadAssembliesAsync(new[] { "SomeAssembly.dll" });
            _lazyLoadedAssemblies.AddRange(assemblies);
        }
#endif
    }
}
