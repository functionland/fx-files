namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IArtifactThumbnailService<TFileService>
    where TFileService : IFileService
{
    Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);
}
