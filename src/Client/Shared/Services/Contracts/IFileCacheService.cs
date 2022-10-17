namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFileCacheService
{
    Task<string?> GetOrCreateCachedFileAsync(string categoryFolder, string uniqueFileName, Func<string, Task<bool>> onCreateFileAsync, CancellationToken? cancellationToken = null);
}
