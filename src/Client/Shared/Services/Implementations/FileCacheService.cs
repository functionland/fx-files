namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class FileCacheService : IFileCacheService
{
    public async Task InitAsync()
    {
        var destinationDirectory = Path.Combine(GetAppCacheDirectory(), "FxThumbs");

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        var cacheCategoryTypes = Enum.GetNames<CacheCategoryType>();
        foreach (var cacheCategoryType in cacheCategoryTypes)
        {
            var categoryDestinationDirectory = Path.Combine(GetAppCacheDirectory(), "FxThumbs", cacheCategoryType);

            if (!Directory.Exists(categoryDestinationDirectory))
            {
                Directory.CreateDirectory(categoryDestinationDirectory);
            }
        }
    }

    public async Task<string?> GetOrCreateCachedFileAsync(CacheCategoryType cacheCategoryType, string cacheKey, Func<string, Task<bool>> onCreateFileAsync, CancellationToken? cancellationToken = null)
    {
        var filePath = Path.Combine(GetAppCacheDirectory(), cacheCategoryType.ToString(), cacheKey);

        if (File.Exists(filePath)) return filePath;

        var isCreated = await onCreateFileAsync(filePath);
        if (!isCreated) return null;

        return filePath;
    }

    protected abstract string GetAppCacheDirectory();
}
