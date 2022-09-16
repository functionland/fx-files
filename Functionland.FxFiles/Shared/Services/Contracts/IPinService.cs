namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IPinService
    {
        Task InitializeAsync();//Search for changes in files and folders, subscribe for watcher event
        Task SetArtifactPinAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
        Task SetArtifactUnPinAsync(string path, CancellationToken? cancellationToken = null);
        IAsyncEnumerable<FsArtifact> GetPinnedArtifactsAsync(string? fullPath);
    }
}
