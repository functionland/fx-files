using System.Reflection;
using Microsoft.Extensions.FileProviders;

#if Android
using Functionland.FxFiles.App.Platforms.Android.Implementations;
#elif Windows
using Functionland.FxFiles.App.Platforms.Windows.Implementations;
#elif iOS
//TODO:using Functionland.FxFiles.App.Platforms.iOS.Implementations;
#endif

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
#elif iOS
        //TODO: services.AddScoped<IFileService, IosFileService>();
#endif

        services.AddAppServices();

        return builder;
    }
}
