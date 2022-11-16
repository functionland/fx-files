namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ThumbnailService
{
    public IFileCacheService FileCacheService { get; set; } = default!;
    public IEnumerable<IThumbnailPlugin> ThumbnailPlugins { get; set; }

    public ThumbnailService(IFileCacheService fileCacheService, IEnumerable<IThumbnailPlugin> thumbnailPlugins)
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
    protected async Task<string?> GetOrCreateThumbnailAsync(
        CacheCategoryType cacheCategoryType,
        ThumbnailScale thumbnailScale,
        string uniqueName,
        Func<Task<Stream>>? getFileStreamFunc,
        string? filePath, CancellationToken? cancellationToken = null)
    {
        var cacheUniqueName = Path.ChangeExtension(uniqueName, "jpg");
        return await FileCacheService.GetOrCreateCachedFileAsync(
            cacheCategoryType,
            cacheUniqueName,
            async (cacheFilePath) => await OnCreateThumbnailAsync(
                                                uniqueName,
                                                thumbnailScale,
                                                cacheFilePath,
                                                getFileStreamFunc,
                                                filePath,
                                                cancellationToken),
            cancellationToken);
    }

    private async Task<bool> OnCreateThumbnailAsync(
        string uniqueFileName,
        ThumbnailScale thumbnailScale,
        string thumbnailFilePath,
        Func<Task<Stream>>? getFileStreamFunc,
        string? filePath,
        CancellationToken? cancellationToken = null)
    {

        if (getFileStreamFunc is null && filePath is null)
            throw new InvalidOperationException("Both stream and filePath are null, which is not valid to create a thumbnail.");

        var plugin = GetRelatedPlugin(uniqueFileName);

        if (plugin is null)
            return false;

        Stream? stream = null;

        try
        {
            if (getFileStreamFunc is not null && !plugin.IsJustFilePathSupported)
                stream = await getFileStreamFunc();

            var thumbnailStream = await plugin.CreateThumbnailAsync(stream, filePath, thumbnailScale, cancellationToken);

            // write stream
            using (var fileStream = File.Create(thumbnailFilePath))
            {
                thumbnailStream.Seek(0, SeekOrigin.Begin);
                thumbnailStream.CopyTo(fileStream);
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            if (stream is not null)
            {
                await stream.DisposeAsync().AsTask();
            }
        }

        return true;
    }

    protected virtual IThumbnailPlugin? GetRelatedPlugin(string uri)
    {
        var extension = Path.GetExtension(uri);
        var plugin = ThumbnailPlugins.FirstOrDefault(plugin => plugin.IsSupported(extension));
        return plugin;
    }
}
