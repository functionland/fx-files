namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class LocalDeviceShareService : ILocalDeviceShareService
{
    public Task EnsureInitializedAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedFsArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareFsArtifactAsync(IEnumerable<string> dids, FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UnShareFsArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UnShareFsArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
