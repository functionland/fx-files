namespace Functionland.FxFiles.Shared.Services.Contracts
{
    public interface IPinService
    {
        Task SetFolderPinAsync(string path, bool isPinned, CancellationToken? cancellationToken = null);
        Task SetFilePinAsync(string path, bool isPinned, CancellationToken? cancellationToken = null);
        Task<FsArtifact> GetPinnedArtifactsAsync();
    }
}
