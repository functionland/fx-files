using System.Reflection;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using Microsoft.Extensions.FileProviders;

#if Windows
using Functionland.FxFiles.App.Platforms.Windows.Implementations;
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

        services.AddAppServices();

#if Windows
        services.AddScoped<IPlatformTestService, WindowsPlatformTestService>();
#endif
        services.AddTransient<FakeFileServicePlatformTest>();

        return builder;
    }
}
