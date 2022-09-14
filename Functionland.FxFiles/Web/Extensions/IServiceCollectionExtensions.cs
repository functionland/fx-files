
using Functionland.FxFiles.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<ThemeInterop>();
        services.AddScoped<IExceptionHandler, ExceptionHandler>();

#if Android
        services.AddScoped<IFileService, Functionland.FxFiles.App.Platforms.Android.Implementations.AndroidFileService>();
        services.AddScoped<IPlatformTestService, Functionland.FxFiles.App.Platforms.Android.Implementations.AndroidPlatformTestService>();
#elif Windows
        services.AddScoped<IFileService, Functionland.FxFiles.App.Platforms.Windows.Implementations.WindowsFileService>();
        services.AddScoped<IPlatformTestService, Functionland.FxFiles.App.Platforms.Windows.Implementations.WindowsPlatformTestService>();
#elif iOS
        //TODO: services.AddScoped<IFileService, IosFileService>();
        services.AddScoped<IPlatformTestService, Functionland.FxFiles.App.Platforms.iOS.Implementations.IosPlatformTestService>();
#endif

        services.AddTransient<FakeFileServicePlatformTest>();

        string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FX\\FxDB.db")};";

        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));

        return services;
    }
}
