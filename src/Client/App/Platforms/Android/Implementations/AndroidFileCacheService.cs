namespace Functionland.FxFiles.Client.App.Platforms.Android.Implementations
{
    public class AndroidFileCacheService : FileCacheService
    {
        public override string GetAppCacheDirectory() =>MauiApplication.Current.CacheDir.Path;
    }
}
