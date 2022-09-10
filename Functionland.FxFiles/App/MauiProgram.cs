using System.Reflection;
using Functionland.FxFiles.App.Platforms.Android.Implementations;
using Microsoft.Extensions.FileProviders;

namespace Functionland.FxFiles.App;

public static class MauiProgram
{
    public static MauiAppBuilder CreateMauiAppBuilder()
    {
#if !BlazorHybrid
        throw new InvalidOperationException("Please switch to blazor hybrid as described in readme.md");
#endif

        var builder = MauiApp.CreateBuilder();
        var assembly = typeof(MauiProgram).GetTypeInfo().Assembly;

        builder
            .UseMauiApp<App>()
            .Configuration.AddJsonFile(new EmbeddedFileProvider(assembly), "wwwroot.appsettings.json", optional: false, false);

        var services = builder.Services;

        services.AddMauiBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

#if Android
        services.AddScoped<IFileService, AndroidFileService>();
#elif Windows
        services.AddScoped<IFileService, WindowsFileService>();
#endif

        services.AddAppServices();

        return builder;
    }
}
