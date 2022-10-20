namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IThumbnailPlugin
{
    bool IsExtensionSupported(string extension, Stream? stream, string? filePath);
    Task<Stream> CreateThumbnailAsync(Stream? input, string? filePath, CancellationToken? cancellationToken = null);
}
