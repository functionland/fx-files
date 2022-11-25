namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IFileWatchService
    {
        void WatchArtifact(FsArtifact fsArtifact);
        void UnWatchArtifact(FsArtifact fsArtifact);
        void UpdateFileWatchCatch(string newPath, string oldPath);
    }

}
