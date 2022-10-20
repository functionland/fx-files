namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class PdfThumbnailPlugin : IThumbnailPlugin
{
    public virtual bool IsJustFilePathSupported => false;

    public async Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null)
    {
        return await OnCreateThumbnailAsync(stream, filePath, thumbnailScale, cancellationToken);
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);

    public bool IsSupported(string extension)
    {
        return new string[] { ".pdf" }.Contains(extension.ToLower());
    }
}
