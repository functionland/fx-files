namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ImageThumbnailPlugin : IThumbnailPlugin
{
    private static List<ThumbnailPluginSupportType> ThumbnailPluginSupportTypes
      => new()
      {
            ThumbnailPluginSupportType.FilePath,
            ThumbnailPluginSupportType.Stream
      };
    public Task<Stream> CreateThumbnailAsync(Stream? input, string? filePath, CancellationToken? cancellationToken = null)
    {
        return OnCreateThumbnailAsync(input, filePath, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? input, string? filePath, CancellationToken? cancellationToken = null);

    public virtual bool IsExtensionSupported(string extension, Stream? stream, string? filePath)
    {
        var isExtentionSupported = new string[] { ".jpg", ".png" }.Contains(extension.ToLower());

        if (!isExtentionSupported) return false;

        return (ThumbnailPluginSupportTypes.Contains(ThumbnailPluginSupportType.Stream) && stream is not null) ||
               (ThumbnailPluginSupportTypes.Contains(ThumbnailPluginSupportType.FilePath) && !string.IsNullOrWhiteSpace(filePath));
    }
}
