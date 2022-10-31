using Functionland.FxFiles.Client.Shared.Utils;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class VideoThumbnailPlugin : IThumbnailPlugin
{
    public virtual bool IsJustFilePathSupported => false;

    public async Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        return await OnCreateThumbnailAsync(stream, filePath, thumbnailScale, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);

    public virtual bool IsSupported(string extension)
    {
        return new string[]
        {
            ".mkv",
            ".mov",
            ".avi",
            ".wmv",
            ".asf",
            ".3gp",
            ".webm",
            ".m4v",
            ".mp4"
        }.Contains(extension.ToLower());
    }
}
