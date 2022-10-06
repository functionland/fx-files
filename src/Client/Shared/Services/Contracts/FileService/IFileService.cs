namespace Functionland.FxFiles.Client.Shared.Services.Contracts
{
    public interface IFileService
    {
        Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null);
        Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null);
        Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null);
        Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null);
        Task MoveArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null);
        Task CopyArtifactsAsync(FsArtifact[] artifacts, string destination, bool overwrite = false, CancellationToken? cancellationToken = null);
        Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null);
        Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null);
        Task DeleteArtifactsAsync(FsArtifact[] artifacts, CancellationToken? cancellationToken = null);
        IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);
        // Todo: Remove Fs
        Task<FsArtifact> GetFsArtifactAsync(string? path, CancellationToken? cancellationToken = null);
        Task<List<FsArtifactChanges>> CheckPathExistsAsync(List<string?> paths, CancellationToken? cancellationToken = null);
        Task FillArtifactMetaAsync(FsArtifact artifact, CancellationToken? cancellationToken = null);
        Task<List<FsArtifactActivity>> GetArtifactActivityHistoryAsync(string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);
    }
}
