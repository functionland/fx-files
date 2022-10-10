using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaFileClient
{
    Task UploadFileAsync(string token, string path, Stream stream, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task AddFolderAsync(string token, string path, string folderName, CancellationToken? cancellationToken = null);
    Task<Stream> GetFileStreamAsync(string token, string filePath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task MoveArtifactsAsync(string token, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task CopyArtifactsAsync(string token, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    Task RenameFileAsync(string token, string filePath, string newName, CancellationToken? cancellationToken = null);
    Task RenameFolderAsync(string token, string folderPath, string newName, CancellationToken? cancellationToken = null);
    Task DeleteArtifactsAsync(string token, IEnumerable<string> sourcesPath, Action<ProgressInfo>? onProgress = null, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string token, string? path = null, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> SearchArtifactsAsync(string token, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);

    Task<FsArtifact> GetArtifactAsync(string token, string? path = null, CancellationToken? cancellationToken = null);

    Task ShareFolderAsync(string token, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFoldersAsync(string token, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task ShareFileAsync(string token, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFilesAsync(string token, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task RevokeShareFolderAsync(string token, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task RevokeShareFoldersAsync(string token, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);
    Task RevokeShareFileAsync(string token, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task RevokeShareFilesAsync(string token, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    IAsyncEnumerable<FsArtifact> GetSharedByMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> GetSharedWithMeArtifacsAsync(string token, CancellationToken? cancellationToken = null);

    Task<FsArtifact> GetArtifactMetaAsync(string token, string path, CancellationToken? cancellationToken = null);
    Task<List<FsArtifactActivity>> GetActivityHistoryAsync(string token, string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);

    Task<string> GetLinkForShareAsync(string token, string path, CancellationToken? cancellationToken = null);
}
