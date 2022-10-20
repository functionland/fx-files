namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IThumbnailPlugin
{
    bool IsSupported(string extension, ThumbnailSourceType sourceType);
    Task<Stream> CreateThumbnailAsync(Stream? inputStream, string? filePath, CancellationToken? cancellationToken = null);
}
