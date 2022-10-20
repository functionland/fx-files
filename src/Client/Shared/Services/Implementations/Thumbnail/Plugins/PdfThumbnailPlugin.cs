namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class PdfThumbnailPlugin : IThumbnailPlugin
{
    private static List<ThumbnailPluginSupportType> ThumbnailPluginSupportTypes
        => new()
        {
            ThumbnailPluginSupportType.FilePath,
            ThumbnailPluginSupportType.Stream
        };

    public Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public bool IsExtensionSupported(string extension, Stream? stream, string? filePath)
    {
        var isExtentionSupported = new string[]
        {
            ".pdf",
        }.Contains(extension.ToLower());

        if (!isExtentionSupported) return false;

        return (ThumbnailPluginSupportTypes.Contains(ThumbnailPluginSupportType.Stream) && stream is not null) ||
               (ThumbnailPluginSupportTypes.Contains(ThumbnailPluginSupportType.FilePath) && !string.IsNullOrWhiteSpace(filePath));

    }
}
