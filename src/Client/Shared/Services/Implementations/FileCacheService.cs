namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

// ToDo: Make platform-specific implementations
// Move classes and interfaces to proper files.
public abstract class FileCacheService : IFileCacheService
{
    public async Task InitAsync()
    {
        var destinationDirectory = Path.Combine(GetAppCacheDirectory(), "FxThumbs");

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        // ToDo: Add folders based on enums.
    }

    public async Task<string?> GetOrCreateCachedFileAsync(string categoryFolder, string cacheKey, Func<string, Task<bool>> onCreateFileAsync, CancellationToken? cancellationToken = null)
    {
        var cacheFolder = Path.Combine(GetAppCacheDirectory(), categoryFolder);
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
        }

        var filePath = Path.Combine(cacheFolder, cacheKey);

        if (!File.Exists(filePath))
        {
            var isCreated = await onCreateFileAsync(filePath);

            if (!isCreated)
                return null;
        }

        return filePath;
    }

    protected abstract string GetAppCacheDirectory();
}
