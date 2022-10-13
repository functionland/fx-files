namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FulaShareService : IFulaShareService
{
    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<ArtifactUserPermission>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsSahredByMeAsync(string path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SetPermissionArtifactsAsync(IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
