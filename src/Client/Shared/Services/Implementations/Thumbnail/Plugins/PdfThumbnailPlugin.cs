namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public abstract class PdfThumbnailPlugin : IThumbnailPlugin
{
    public virtual bool IsJustFilePathSupported => false;

    public Task<Stream> CreateThumbnailAsync(Stream? stream, string? filePath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    protected abstract Task<Stream> OnCreateThumbnailAsync(Stream? stream, string? filePath, CancellationToken? cancellationToken = null);

    public bool IsSupported(string extension)
    {
        return new string[] { ".pdf" }.Contains(extension.ToLower());
    }
}
