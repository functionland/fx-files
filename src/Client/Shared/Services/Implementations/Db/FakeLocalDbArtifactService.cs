namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeLocalDbArtifactService : ILocalDbArtifactService
{
    public FakeLocalDbArtifactService(List<FsArtifact> artifacts)
    {

    }

    public Task<FsArtifact> GetArtifactAsync(string localPath, string userToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath, string userToken)
    {
        throw new NotImplementedException();
    }

    public Task<FsArtifact> CreateArtifactAsync(FsArtifact fsArtifact, ArtifactUploadStatus uploadStatus, string localPath, string userToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveArtifactAsync(string localPath, string userToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateFileAsync(FsArtifact fsArtifact, string localPath, string userToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateFolderAsync(FsArtifact fsArtifact, string localPath, string userToken)
    {
        throw new NotImplementedException();
    }
}
