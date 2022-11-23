namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaSyncService
{
    Task InitAsync(FulaUser fulaUser, CancellationToken? cancellationToken = null);
    Task StartSyncAsync(CancellationToken? cancellationToken = null);
    Task StopSyncAsync(CancellationToken? cancellationToken = null);
    Task EnsureInitializedAsync(CancellationToken? cancellationToken = null);
    Task<List<FsArtifact>> SyncItemsAsync(CancellationToken? cancellationToken = null);
    Task<List<FsArtifact>> SyncContentsAsync(CancellationToken? cancellationToken = null);
    Task SyncContentAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
