namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IThumbnailPlugin
{
    bool IsExtensionSupported(string extension);
    Task<Stream> CreateThumbnailAsync(Stream input);
}
