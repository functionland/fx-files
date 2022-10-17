namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IArtifactThumbnailService
{
    Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
