using System.Reflection;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using Microsoft.Extensions.FileProviders;
using Functionland.FxFiles.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Shared.Services;

#if Windows
using Functionland.FxFiles.App.Platforms.Windows.Implementations;
#elif iOS
using Functionland.FxFiles.App.Platforms.iOS.Implementations;
#elif Android
using Functionland.FxFiles.App.Platforms.Android.Implementations;
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

#if Windows
        services.AddScoped<IPlatformTestService, WindowsPlatformTestService>();
#elif Android
        services.AddScoped<IPlatformTestService, AndroidPlatformTestService>();
#elif iOS
        services.AddScoped<IPlatformTestService, IosPlatformTestService>();
#endif
        services.AddTransient<FakeFileServicePlatformTest>();

        string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FX\\FxDB.db")};";

        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));
        return builder;
    }
}
