using Functionland.FxFiles.Client.Shared.Components.Modal;

namespace Functionland.FxFiles.Client.Shared.Services.Implementations;

public partial class FulaFileService : IFulaFileService
{
    public virtual Task<List<(string Path, bool IsExist)>> CheckPathExistsAsync(IEnumerable<string?> paths,
        CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<(FsArtifact artifact, Exception exception)>> CopyArtifactsAsync(IList<FsArtifact> artifacts, string destination, Func<FsArtifact, Task<bool>>? onShouldOverwrite = null, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<FsArtifact> CreateFileAsync(string path, Stream stream, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<List<FsArtifact>> CreateFilesAsync(IEnumerable<(string path, Stream stream)> files, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<FsArtifact> CreateFolderAsync(string path, string folderName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task DeleteArtifactsAsync(IList<FsArtifact> artifacts, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string? path = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<Stream> GetFileContentAsync(string filePath, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task<FsArtifact> GetArtifactAsync(string? path, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<(FsArtifact artifact, Exception exception)>> MoveArtifactsAsync(IList<FsArtifact> artifacts, string destination, Func<FsArtifact, Task<bool>>? onShouldOverwrite = null, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task RenameFileAsync(string filePath, string newName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public virtual Task RenameFolderAsync(string folderPath, string newName, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task FillArtifactMetaAsync(FsArtifact fsArtifact, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<FsArtifactActivity>> GetArtifactActivityHistoryAsync(string path, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FsArtifact> GetSearchArtifactAsync(DeepSearchFilter? deepSearchFilter, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetArtifactSizeAsync(string path, Action<long>? onProgress, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }

    public string GetShowablePath(string artifactPath)
    {
        throw new NotImplementedException();
    }

    public Task CopyFileAsync(FsArtifact artifact, string destinationFullPath, Func<FsArtifact, Task<bool>>? onShouldOverwrite = null, Func<ProgressInfo, Task>? onProgress = null, CancellationToken? cancellationToken = null)
    {
        throw new NotImplementedException();
    }
}
