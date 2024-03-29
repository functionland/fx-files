﻿
using Functionland.FxFiles.Client.Shared.Infra;

namespace Functionland.FxFiles.Client.Shared.Components;

public partial class BottomNavigation
{
    public string CurrentUrl { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

#if MultilingualEnabled
    protected async override Task OnAfterFirstRenderAsync()
    {
#if Maui
        var preferredCultureCookie = Preferences.Get(".AspNetCore.Culture", null);
#else
        var preferredCultureCookie = await JSRuntime.InvokeAsync<string?>("window.App.getCookie", ".AspNetCore.Culture");
#endif
        SelectedCulture = CultureInfoManager.GetCurrentCulture(preferredCultureCookie);

        StateHasChanged();

        await base.OnAfterFirstRenderAsync();
    }
#endif

    string? SelectedCulture;

    async Task OnCultureChanged()
    {
        var cultureCookie = $"c={SelectedCulture}|uic={SelectedCulture}";

#if Maui
        Preferences.Set(".AspNetCore.Culture", cultureCookie);
#else
        await JSRuntime.InvokeVoidAsync("window.App.setCookie", ".AspNetCore.Culture", cultureCookie, 30 * 24 * 3600);
#endif

        NavigationManager.ForceReload();
    }

    List<BitDropDownItem> GetCultures()

    {
        return CultureInfoManager.SupportedCultures
            .Select(sc => new BitDropDownItem { Value = sc.code, Text = sc.name })
            .ToList();
    }
}
