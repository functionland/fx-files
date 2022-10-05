namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakePinService : ILocalDevicePinService, IFulaPinService
{
    //TODO: Complete fake implementation
    private readonly List<FsArtifact> _fsArtifacts;

    public FakePinService(IEnumerable<FsArtifact> fsArtifacts)
    {
        _fsArtifacts = fsArtifacts.ToList();
    }

    public Task EnsureInitializedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifact>> GetPinnedArtifactsAsync()
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public bool IsPinned(FsArtifact fsArtifact)
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
