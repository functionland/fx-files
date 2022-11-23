namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface ILocalDbArtifactService
{
    Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath, string userToken);

    Task<FsArtifact> GetArtifactAsync(string localPath, string userToken);

    Task<FsArtifact> CreateArtifactAsync(FsArtifact fsArtifact, ArtifactPersistenceStatus uploadStatus, string localPath, string userToken);

    Task RemoveArtifactAsync(string localPath, string userToken);


    Task UpdateFileAsync(FsArtifact fsArtifact, string localPath, string userToken);

    Task UpdateFolderAsync(FsArtifact fsArtifact, string localPath, string userToken);

    // TODO: Task<bool> CheckIfPathExsit(string localPath);
}
