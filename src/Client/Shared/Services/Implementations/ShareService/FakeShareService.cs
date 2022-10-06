namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeShareService : IFulaShareService, ILocalDeviceShareService
{
    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FulaUser>> GetArtifactSharesAsync(string path, CancellationToken? cancellationToken = null)
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

    public Task RevokeShareArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task RevokeShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareArtifactAsync(IEnumerable<string> dids, FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
