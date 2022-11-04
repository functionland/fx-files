namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IArtifactThumbnailService<out TFileService>
    where TFileService : IFileService
{
    Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, ThumbnailScale thumbnailScale, CancellationToken? cancellationToken = null);
}
