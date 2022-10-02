﻿using Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;
using Functionland.FxFiles.Client.App.Platforms.iOS.Implementations.Test;
using Functionland.FxFiles.Client.Shared.TestInfra.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class IiOSServiceCollectionExtensions
{
    public static IServiceCollection AddClientiOSServices(this IServiceCollection services)
    {
        // Services being registered here can get injected in iOS.

        //TODO: services.AddSingleton<IFileService, IosFileService>();
        //services.AddTransient<IPlatformTestService, IosPlatformTestService>();
        //services.AddTransient<IosFileServicePlatformTest>();
        services.AddSingleton<IFileService, IosFileService>();
        services.AddSingleton<IPlatformTestService, IosPlatformTestService>();
        
        services.AddSingleton<IThumbnailService, IosThumbnailService>();
        services.AddSingleton<IFileWatchService, IosFileWatchService>();
        return services;
    }
}
