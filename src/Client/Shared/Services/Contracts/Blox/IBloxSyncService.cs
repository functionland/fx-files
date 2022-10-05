namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface IBloxSyncService
{
    Task<List<BloxSyncItem>> SyncItemsAsync();
    Task<List<FsArtifact>> SyncContentsAsync();
    Task SyncContentAsync(FsArtifact artifact);
}
