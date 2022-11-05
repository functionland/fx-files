namespace Functionland.FxFiles.Client.App.Platforms.iOS.Implementations;

public class IosFileCacheService : FileCacheService
{
    public override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
