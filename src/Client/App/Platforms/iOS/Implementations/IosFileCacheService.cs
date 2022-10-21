namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosFileCacheService : FileCacheService
{
    protected override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
