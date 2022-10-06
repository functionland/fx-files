namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FulaShareService : IFulaShareService
{
    public Task EnsureInitializedAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedByMeArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task InitAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareArtifactAsync(IEnumerable<string> dids, FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task ShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<FsArtifact> fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UnShareArtifactAsync(IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UnShareArtifactsAsync(IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FulaUser>> WhoHasAccessToArtifact(string artifactFullPath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
