namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

internal class WindowsFileCacheService : FileCacheService
{
    public override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
