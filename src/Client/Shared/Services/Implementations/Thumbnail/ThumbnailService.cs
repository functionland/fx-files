using Functionland.FxFiles.Client.Shared.Models;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ThumbnailService : IThumbnailService
{
    public IFileCacheService FileCacheService { get; set; } = default!;
    public IThumbnailPlugin[] ThumbnailPlugins { get; set; }

    public ThumbnailService(IFileCacheService fileCacheService, IThumbnailPlugin[] thumbnailPlugins)
    {
        FileCacheService = fileCacheService;
        ThumbnailPlugins = thumbnailPlugins;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uniqueFileName">adfadgfasdfasdf52465s4fd6as5f4fa6sd5f4as6d5f.pdf</param>
    /// <param name="fileStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>{cache}/adfadgfasdfasdf52465s4fd6as5f4fa6sd5f4as6d5f.jpg</returns>
    protected async Task<string?> GetOrCreateThumbnailAsync(string categoryFolder, string uniqueName, Func<Task<Stream>> getFileStreamFunc, CancellationToken? cancellationToken = null)
    {
        var cacheUniqueName = Path.ChangeExtension(uniqueName, "jpg");
        return await FileCacheService.GetOrCreateCachedFileAsync(
            categoryFolder,
            cacheUniqueName,
            async (cacheFilePath) => await OnCreateThumbnailAsync(uniqueName, cacheFilePath, await getFileStreamFunc(), cancellationToken),
            cancellationToken);
    }

    private async Task<bool> OnCreateThumbnailAsync(string uniqueFileName, string thumbnailFilePath, Stream stream, CancellationToken? cancellationToken = null)
    {
        var plugin = GetRelatedPlugin(uniqueFileName);
        if (plugin is null)
            return false;

        var thumbnailStream = await plugin.CreateThumbnailAsync(stream);

        // write stream
        using (var fileStream = File.Create(thumbnailFilePath))
        {
            thumbnailStream.Seek(0, SeekOrigin.Begin);
            thumbnailStream.CopyTo(fileStream);
        }

        return true;
    }

    protected virtual IThumbnailPlugin? GetRelatedPlugin(string uri)
    {
        var extension = Path.GetExtension(uri);
        var plugin = ThumbnailPlugins.FirstOrDefault(plugin => plugin.IsExtensionSupported(extension));
        return plugin;
    }

    //public abstract Task<string> MakeThumbnailAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null);

    //protected abstract string GetAppCacheDirectory();


}
