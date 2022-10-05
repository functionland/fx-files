using Functionland.FxFiles.Client.Shared.Services.FulaClient.Common;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaFileClient
{
    /// <summary>
    /// Key is parent full path.
    /// </summary>
    Dictionary<string, EventHandler<List<FileProgressEventArgs>>> EventHandlers { get; protected set; }

    Task UploadFileAsync(DIdDocument dIdDocument, string path, Stream stream, CancellationToken cancellationToken);
    Task UploadFilesAsync(DIdDocument dIdDocument, IEnumerable<(string path, Stream stream)> files, CancellationToken cancellationToken);
    Task AddFolderAsync(DIdDocument dIdDocument, string path, string folderName, CancellationToken cancellationToken);
    Task<Stream> DownloadFileAsync(DIdDocument dIdDocument, string filePath, CancellationToken? cancellationToken = null);
    Task MoveArtifactsAsync(DIdDocument dIdDocument, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task CopyArtifactsAsync(DIdDocument dIdDocument, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task RenameFileAsync(DIdDocument dIdDocument, string filePath, string newName, CancellationToken? cancellationToken = null);
    Task RenameFolderAsync(DIdDocument dIdDocument, string folderPath, string newName, CancellationToken? cancellationToken = null);
    Task DeleteArtifactsAsync(DIdDocument dIdDocument, IEnumerable<string> sourcesPath, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> GetArtifactsAsync(DIdDocument dIdDocument, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);

    Task ShareFolderAsync(DIdDocument dIdDocument, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFoldersAsync(DIdDocument dIdDocument, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task ShareFileAsync(DIdDocument dIdDocument, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFilesAsync(DIdDocument dIdDocument, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task UnShareFolderAsync(DIdDocument dIdDocument, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task UnShareFoldersAsync(DIdDocument dIdDocument, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    Task UnShareFileAsync(DIdDocument dIdDocument, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task UnShareFilesAsync(DIdDocument dIdDocument, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    IAsyncEnumerable<FsArtifact> GetSharedArtifacsAsync(DIdDocument dIdDocument, CancellationToken? cancellationToken = null);

    Task<FsArtifact> GetFsArtifactMetaAsync(DIdDocument dIdDocument, string fullPath, long? page = null, long? pageSize = null, CancellationToken? cancellationToken = null);
}
