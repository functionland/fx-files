using Functionland.FxFiles.Client.Shared.Services.Implementations;

namespace Functionland.FxFiles.Client.Test.Services.Implementations;

internal class FakeFileCacheService : FileCacheService
{
    private readonly string _cacheFolder;

    public FakeFileCacheService(string cacheFolder)
    {
        _cacheFolder = cacheFolder;
    }
    public override string GetAppCacheDirectory() => _cacheFolder;
}