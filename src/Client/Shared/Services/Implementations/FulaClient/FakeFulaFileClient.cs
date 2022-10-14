using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public class FakeFulaFileClient : IFulaFileClient
{
    public FakeFulaFileClient(List<FsArtifact> artifacts)
    {

    }

    public Task AddFolderAsync(string token, string path, string folderName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> CopyArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteArtifactsAsync(string token, IEnumerable<string> sourcesPath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetChildrenArtifactsAsync(string token, string? path = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifacsAsync(string token, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> MoveArtifactsAsync(string token, IEnumerable<string> sourcePaths, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> SearchArtifactsAsync(string token, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task SetPermissionArtifactsAsync(string token, IEnumerable<ArtifactPermissionInfo> permissionInfos, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UpdateFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task UploadFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
