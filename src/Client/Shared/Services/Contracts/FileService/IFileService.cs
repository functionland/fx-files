using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IFileService
    {
        Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null);
        Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null);
        Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null);
        Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null);
        Task MoveArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null);
        Task CopyArtifactsAsync(IList<FsArtifact> artifacts, string destination, bool overwrite = false, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null);
        Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null);
        Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null);
        Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null);
        IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null);
        Task<FsArtifact> GetArtifactAsync(string? path, CancellationToken? cancellationToken = null);
        IAsyncEnumerable<FsArtifact> GetSearchArtifactAsync(DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null);
        Task<List<FsArtifactChanges>> CheckPathExistsAsync(List<string?> paths, CancellationToken? cancellationToken = null);
        Task FillArtifactMetaAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
        Task<List<FsArtifactActivity>> GetArtifactActivityHistoryAsync(string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);
    }
}
