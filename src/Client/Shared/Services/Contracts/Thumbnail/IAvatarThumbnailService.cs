namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IAvatarThumbnailService
{
    Task<string?> GetOrCreateThumbnailAsync(string did, CancellationToken? cancellationToken = null);
}
