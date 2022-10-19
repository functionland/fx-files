using System.Reflection;

using Microsoft.AspNetCore.Components.Routing;

namespace Functionland.FxFiles.Client.Shared;

public partial class App
{
#if BlazorWebAssembly && !BlazorHybrid
    private List<Assembly> _lazyLoadedAssemblies = new();
    [AutoInject] private Microsoft.AspNetCore.Components.WebAssembly.Services.LazyAssemblyLoader _assemblyLoader = default!;
#endif

    [AutoInject] private IJSRuntime _jsRuntime = default!;

    [AutoInject] private ThemeInterop ThemeInterop = default!;

    private FxTheme CurrentTheme;

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CurrentTheme = await ThemeInterop.GetThemeAsync();
            await ThemeInterop.ApplyThemeAsync(CurrentTheme);
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnNavigateAsync(NavigationContext args)
    {
        // Blazor Server & Pre Rendering use created cultures in UseRequestLocalization middleware
        // Android, windows and iOS have to set culture programmatically.
        // Browser is gets handled in Web project's Program.cs\
        // Browser is gets handled in Web project's Program.cs
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
        _isLoading = false;
    }
    override protected async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
