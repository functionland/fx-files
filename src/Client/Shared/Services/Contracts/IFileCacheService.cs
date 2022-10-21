namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileCacheService
{
    Task InitAsync();
    Task<string?> GetOrCreateCachedFileAsync(CacheCategoryType cacheCategoryType, string uniqueFileName, Func<string, Task<bool>> onCreateFileAsync, CancellationToken? cancellationToken = null);
}
