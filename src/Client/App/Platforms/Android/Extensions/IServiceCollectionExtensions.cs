using Functionland.FxFiles.Client.App.Platforms.Android.Contracts;
using Functionland.FxFiles.Client.App.Platforms.Android.Implementations;
using Functionland.FxFiles.Client.App.Platforms.Android.Implementations.Test;
using Functionland.FxFiles.Client.App.Platforms.Android.PermissionsUtility;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Microsoft.Extensions.DependencyInjection;

public static class IAndroidServiceCollectionExtensions
{
    public static IServiceCollection AddClientAndroidServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in Android.


        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
        {
            services.AddSingleton<ILocalDeviceFileService, Android11andAboveFileService>();
            services.AddSingleton<IPermissionUtils, Android11andAbovePermissionUtils>();
        }
        else
        {
            services.AddSingleton<ILocalDeviceFileService, Android10FileService>();
            services.AddSingleton<IPermissionUtils, Android10PermissionUtils>();
        }

        services.AddSingleton<IPlatformTestService, AndroidPlatformTestService>();

        // FileService Platform Tests
        services.AddTransient<InternalAndroidFileServicePlatformTest>();
        services.AddTransient<ExternalAndroidFileServicePlatformTest>();

        // Thumbnail Plugin Platform Tests
        services.AddTransient<AndroidImageThumbnailPluginPlatformTest<ILocalDeviceFileService>>();
        services.AddTransient<AndroidImageThumbnailPluginPlatformTest<IFulaFileService>>();

        services.AddSingleton<IFileWatchService, AndroidFileWatchService>();
        services.AddSingleton<IFileCacheService, AndroidFileCacheService>();

        // Thumbnail Plugins
        services.AddSingleton<IThumbnailPlugin, AndroidImageThumbnailPlugin>();
        services.AddSingleton<IThumbnailPlugin, AndroidVideoThumbnailPlugin>();
        services.AddSingleton<IThumbnailPlugin, AndroidPdfThumbnailPlugin>();

        services.AddSingleton<IZipPathUtilService, AndroidZipPathUtilService>();

        return services;
    }
}
