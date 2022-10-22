namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ImageThumbnailPlugin : IThumbnailPlugin
{
    public bool IsJustFilePathSupported => false;

    public Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        return OnCreateThumbnailAsync(stream, filePath, thumbnailScale, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);

    public bool IsSupported(string extension)
    {
        return new string[]
        {
            ".jpg",
            ".png"
        }.Contains(extension.ToLower());
    }
}
