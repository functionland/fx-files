using Functionland.FxFiles.Client.Shared.Shared;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Functionland.FxFiles.Client.App;

public static class MauiProgram
{
    public static MauiAppBuilder CreateMauiAppBuilder()
    {
#if Windows
  AppCenter.Start("7f2ed707-46a6-480d-bf29-d6f027eaed61",typeof(Analytics), typeof(Crashes));
#else
        AppCenter.Start(
                "ios=8b71c972-bb4b-4429-a8b7-5aae33857d1a;" +
                "android=c6db6bea-416d-4688-9e14-ca8b30af1775;",
               typeof(Analytics), typeof(Crashes));
#endif

#if !BlazorHybrid
        throw new InvalidOperationException("Please switch to blazor hybrid as described in readme.md");
#endif

        var builder = MauiApp.CreateBuilder();
        var assembly = typeof(MainLayout).GetTypeInfo().Assembly;

        builder
            .UseMauiApp<App>()
            .Configuration.AddJsonFile(new EmbeddedFileProvider(assembly), "wwwroot.appsettings.json", optional: false, false);

        var services = builder.Services;

        services.AddMauiBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
        services.AddClientSharedServices();
        services.AddClientAppServices();

        return builder;
    }
}
