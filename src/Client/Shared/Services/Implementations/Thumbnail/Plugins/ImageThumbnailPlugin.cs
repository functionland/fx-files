namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ImageThumbnailPlugin : IThumbnailPlugin
{
    public Task<Stream> CreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
    {
        return OnCreateThumbnailAsync(input, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null);

    public bool IsExtensionSupported(string extension)
    {
        return new string[]
        {
            "jpg",
            "png"
        }.Contains(extension.ToLower());
    }
}
