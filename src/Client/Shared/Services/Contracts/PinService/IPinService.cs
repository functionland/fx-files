namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IPinService
{
    Task InitializeAsync(CancellationToken? cancellationToken = null);
    Task EnsureInitializedAsync(CancellationToken? cancellationToken = null);
    Task SetArtifactsPinAsync(FsArtifact[] artifact, CancellationToken? cancellationToken = null);
    Task SetArtifactsUnPinAsync(string[] path, CancellationToken? cancellationToken = null);
    Task<List<FsArtifact>> GetPinnedArtifactsAsync(CancellationToken? cancellationToken = null);
    Task<bool> IsPinnedAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
