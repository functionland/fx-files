namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FulaPinService : IFulaPinService
{
    public Task EnsureInitializedAsync()
    {
        throw new NotImplementedException();
    }

    //TODO: complete FulaPinService
    public Task<List<FsArtifact>> GetPinnedArtifactsAsync()
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsPinnedAsync(FsArtifact fsArtifact)
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
