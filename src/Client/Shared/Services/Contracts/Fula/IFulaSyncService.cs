namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaSyncService
{
    Task InitAsync(CancellationToken? cancellationToken = null);
    Task EnsureInitializedAsync(CancellationToken? cancellationToken = null);
    Task<List<BloxSyncItem>> SyncItemsAsync(CancellationToken? cancellationToken = null);
    Task<List<FsArtifact>> SyncContentsAsync(CancellationToken? cancellationToken = null);
    Task SyncContentAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
}
