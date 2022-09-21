
using Functionland.FxFiles.Shared.Services;
using Functionland.FxFiles.Shared.Services.Implementations.Db;
using Functionland.FxFiles.Shared.TestInfra.Contracts;
using Functionland.FxFiles.Shared.TestInfra.Implementations;
using Prism.Events;
using EventAggregator = Prism.Events.EventAggregator;
using Functionland.FxFiles.App.Components;

#if Android
using Functionland.FxFiles.App.Platforms.Android.Implementations;
using Functionland.FxFiles.App.Platforms.Android.Implementations.Test;
#elif Windows
using Functionland.FxFiles.App.Platforms.Windows.Implementations;
using Functionland.FxFiles.App.Platforms.Windows.Implementations.Test;
#elif iOS
using Functionland.FxFiles.App.Platforms.iOS.Implementations;
using Functionland.FxFiles.App.Platforms.iOS.Implementations.Test;
#endif

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<ThemeInterop>();
        services.AddScoped<IExceptionHandler, ExceptionHandler>();

#if Android
        services.AddSingleton<IFileService, AndroidFileService>();
        services.AddSingleton<IPlatformTestService, AndroidPlatformTestService>();
        services.AddTransient<InternalAndroidFileServicePlatformTest>();
        services.AddTransient<ExternalAndroidFileServicePlatformTest>();
        services.AddSingleton<IThumbnailService, AndroidThumbnailService>();
#elif Windows
        services.AddSingleton<IFileService, Functionland.FxFiles.App.Platforms.Windows.Implementations.WindowsFileService>();
        services.AddSingleton<IPlatformTestService, WindowsPlatformTestService>();
        services.AddTransient<WindowsFileServicePlatformTest>();
        services.AddSingleton<IThumbnailService, WindowsThumbnailService>();
#elif iOS
        //TODO: services.AddSingleton<IFileService, IosFileService>();
        //services.AddTransient<IPlatformTestService, IosPlatformTestService>();
        //services.AddTransient<IosFileServicePlatformTest>();
#else

        services.AddSingleton<IFileService>((serviceProvider) => FakeFileServiceFactory.CreateSimpleFileListOnRoot(serviceProvider));
        services.AddSingleton<IPlatformTestService, FakePlatformTestService>();
        services.AddTransient<FakeFileServicePlatformTest_CreateTypical>();
        services.AddTransient<FakeFileServicePlatformTest_CreateSimpleFileListOnRoot>();
#endif


        string connectionString = $"DataSource={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "FxDB.db")};";

        services.AddSingleton<IFxLocalDbService, FxLocalDbService>(_ => new FxLocalDbService(connectionString));

        services.AddSingleton<IPinService, PinService>();
        services.AddSingleton<IEventAggregator, EventAggregator>();

        return services;
    }
}
