using System.IO;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeLocalDbArtifactService : ILocalDbArtifactService
{
    private List<FsArtifact> _allArtifacts = new();
    public FakeLocalDbArtifactService(List<FsArtifact> artifacts)
    {
        _allArtifacts.AddRange(artifacts);
    }

    public async Task<FsArtifact> CreateArtifactAsync(FsArtifact fsArtifact, ArtifactUploadStatus uploadStatus, string localPath, string userToken)
    {
        fsArtifact.ArtifactUploadStatus = uploadStatus;
        fsArtifact.LocalFullPath = localPath;
        fsArtifact.UserToken = userToken;

        _allArtifacts.Add(fsArtifact);
        return fsArtifact;
    }

    public async Task<FsArtifact> GetArtifactAsync(string localPath, string userToken)
    {
        var result = _allArtifacts.FirstOrDefault(f => f.FullPath == localPath);

        if (result is null)
            throw new Exception();

        return result;
    }

    public async Task<List<FsArtifact>> GetChildrenArtifactsAsync(string localPath, string userToken)
    {
        return _allArtifacts.Where(f => f.ParentFullPath == localPath).ToList();
    }

    public async Task RemoveArtifactAsync(string localPath, string userToken)
    {
        var toRemove = _allArtifacts.Where(f => f.FullPath == localPath).FirstOrDefault();

        //ToDo: Proper null check for toRemove variable.
        if (toRemove is null) return;
        _allArtifacts.Remove(toRemove);
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
