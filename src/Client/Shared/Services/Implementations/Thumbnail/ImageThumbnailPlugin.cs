namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class ImageThumbnailPlugin : IThumbnailPlugin
{
    public Task<Stream> CreateThumbnailAsync(Stream input)
    {
        return OnCreateThumbnailAsync(input);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream input);

    public bool IsExtensionSupported(string extension)
    {
        return new string[]
        {
            "jpg",
            "png"
        }.Contains(extension.ToLower());
    }
}
