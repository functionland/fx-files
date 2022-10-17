namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class PdfThumbnailPlugin : IThumbnailPlugin
{
    public Task<Stream> CreateThumbnailAsync(Stream input)
    {
        throw new NotImplementedException();
    }

    public bool IsExtensionSupported(string extension)
    {
        return new string[]
        {
            "jpg",
            "png"
        }.Contains(extension.ToLower());
    }
}
