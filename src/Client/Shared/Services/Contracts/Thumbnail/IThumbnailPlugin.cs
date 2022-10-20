namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IThumbnailPlugin
{
    bool IsJustFilePathSupported { get;  }
    bool IsSupported(string extension);
    Task<Stream> CreateThumbnailAsync(Stream? inputStream, string? filePath, CancellationToken? cancellationToken = null);
}
