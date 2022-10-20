namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class PdfThumbnailPlugin : IThumbnailPlugin
{
    public Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, CancellationToken? cancellationToken = null);

    public virtual bool IsSupported(string extension, ThumbnailSourceType sourceType)
    {
        return new string[]
        {
            "jpg",
            "png"
        }.Contains(extension.ToLower());
    }
}
