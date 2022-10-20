namespace Functionland.FxFiles.Client.App.Platforms.Windows.Implementations;

internal class WindowsFileCacheService : FileCacheService
{
    protected override string GetAppCacheDirectory() => FileSystem.CacheDirectory;
}
