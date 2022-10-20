namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations
{
    public class AndroidFileCacheService : FileCacheService
    {
        protected override string GetAppCacheDirectory() => MauiApplication.Current.CacheDir.Path;
    }
}
