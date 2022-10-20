namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IArtifactThumbnailService<TFileService>
    where TFileService : IFileService
{
    Task<string?> GetOrCreateThumbnailAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
