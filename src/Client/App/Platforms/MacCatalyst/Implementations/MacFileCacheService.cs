namespace Functionland.FxFiles.Client.App.Platforms.MacCatalyst.Implementations;

public class MacFileCacheService : FileCacheService
{
    public override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
