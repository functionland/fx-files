using Functionland.FxFiles.Client.Shared.Shared;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui.LifecycleEvents;
using System.Reflection;

namespace Functionland.FxFiles.Client.App;

public static class MauiProgram
{
    public static MauiAppBuilder CreateMauiAppBuilder()
    {
       
#if RELEASE
       
        AppCenter.Start(
            $"windowsdesktop={Configuration.AppCenterWindowsAppSecret};"+
            $"ios={Configuration.AppCenteriOSAppSecret};" +
            $"android={Configuration.AppCenterAndroidAppSecret};",
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
        
        builder.ConfigureLifecycleEvents(lifecycle => {
#if WINDOWS
            lifecycle.AddWindows(windows => windows.OnWindowCreated((del) => {
                del.ExtendsContentIntoTitleBar = false;
                del.Title = "FxFiles";
            }));
#endif
        });

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
