namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IPinService
    {
        Task InitializeAsync();
        Task SetArtifactsPinAsync(FsArtifact[] artifact, CancellationToken? cancellationToken = null);
        Task SetArtifactsUnPinAsync(string[] path, CancellationToken? cancellationToken = null);
        Task<List<FsArtifact>> GetPinnedArtifactsAsync();
        bool IsPinned(FsArtifact fsArtifact);
    }
}
