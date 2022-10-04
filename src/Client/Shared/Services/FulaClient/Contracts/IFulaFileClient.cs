using Functionland.FxFiles.Client.Shared.Services.FulaClient.Common;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaFileClient
{
    /// <summary>
    /// Key is parent full path.
    /// </summary>
    Dictionary<string, EventHandler<List<FileProgressEventArgs>>> EventHandlers { get; protected set; }

    Task UploadFileAsync(string did, string path, Stream stream, CancellationToken cancellationToken);
    Task UploadFilesAsync(string did, IEnumerable<(string path, Stream stream)> files, CancellationToken cancellationToken);
    Task AddFolderAsync(string did, string path, string folderName, CancellationToken cancellationToken);
    Task<Stream> DownloadFileAsync(string did, string filePath, CancellationToken? cancellationToken = null);
    Task MoveArtifactsAsync(string did, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task CopyArtifactsAsync(string did, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task RenameFileAsync(string did, string filePath, string newName, CancellationToken? cancellationToken = null);
    Task RenameFolderAsync(string did, string folderPath, string newName, CancellationToken? cancellationToken = null);
    Task DeleteArtifactsAsync(string did, IEnumerable<string> sourcesPath, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<FsArtifact> GetArtifactsAsync(string did, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);
   
    Task ShareFolderAsync(string did, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFoldersAsync(string did, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task ShareFileAsync(string did, IEnumerable<string> dids, string fullPath, CancellationToken? cancellationToken = null);
    Task ShareFilesAsync(string did, IEnumerable<string> dids, IEnumerable<string> fullPaths, CancellationToken? cancellationToken = null);

    Task UnShareFolderAsync(string did, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task UnShareFoldersAsync(string did, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);

    Task UnShareFileAsync(string did, IEnumerable<string> dids, string artifactFullPath, CancellationToken? cancellationToken = null);
    Task UnShareFilesAsync(string did, IEnumerable<string> dids, IEnumerable<string> artifactFullPaths, CancellationToken? cancellationToken = null);
   
    IAsyncEnumerable<FsArtifact> GetSharedArtifacsAsync(string did, CancellationToken? cancellationToken = null);

    Task<List<FulaUser>> WhoHasAccessToArtifact(string did,string artifactFullPath, CancellationToken? cancellationToken = null);
}
