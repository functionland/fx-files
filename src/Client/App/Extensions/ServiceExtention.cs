namespace Functionland.FxFiles.Client.App.Extensions;

public static class ServiceExtention
{
    public static TService GetRequiredService<TService>()
        where TService: class
    {
        return Current().GetRequiredService<TService>();
    }

    private static IServiceProvider Current()
    {
#if WINDOWS 
        return MauiWinUIApplication.Current.Services;
#elif ANDROID
        return MauiApplication.Current.Services;
#elif IOS || MACCATALYST
        return MauiUIApplicationDelegate.Current.Services;
#else
        throw new InvalidOperationException("this platform is not supported.");
#endif
    }
}
