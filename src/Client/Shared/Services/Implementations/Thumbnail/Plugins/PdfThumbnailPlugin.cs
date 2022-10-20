namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class PdfThumbnailPlugin : IThumbnailPlugin
{
    public Task<Stream> CreateThumbnailAsync(Stream input, CancellationToken? cancellationToken = null)
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
