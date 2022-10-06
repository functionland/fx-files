namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IFulaSyncService
{
    Task InitAsync();
    Task EnsureInitializedAsync();
    Task<List<BloxSyncItem>> SyncItemsAsync();
    Task<List<FsArtifact>> SyncContentsAsync();
    Task SyncContentAsync(FsArtifact artifact);
}
