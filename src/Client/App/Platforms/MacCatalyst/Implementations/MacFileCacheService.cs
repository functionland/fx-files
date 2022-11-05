namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public class MacFileCacheService : FileCacheService
{
    protected override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
