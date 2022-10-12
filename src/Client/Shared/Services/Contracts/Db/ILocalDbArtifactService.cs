namespace Functionland.FxFiles.Client.Shared.Services.Contracts;

public interface ILocalDbArtifactService
{
    Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath);

    Task<FsArtifact> GetArtifactAsync(string localPath);

    //TODO: Create artifact, Remove artifact, Update file, Update folder
}
