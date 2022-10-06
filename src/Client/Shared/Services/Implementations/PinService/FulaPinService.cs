namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FulaPinService : IFulaPinService
{
    public Task EnsureInitializedAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifact>> GetPinnedArtifactsAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync(CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsPinnedAsync(FsArtifact artifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SetArtifactsPinAsync(FsArtifact[] artifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SetArtifactsUnPinAsync(string[] path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
