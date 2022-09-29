using Functionland.FxFiles.Client.Shared.Services.FulaClient.Common;

namespace Functionland.FxFiles.Client.Shared.Services.FulaClient.Contracts;

public interface IFulaFileClient
{
    EventHandler<FileProgressEventArgs> UploadFile(string did, string path, Stream stream, CancellationToken cancellationToken);
    EventHandler<FileProgressEventArgs> UploadFiles(string did, IEnumerable<(string path, Stream stream)> files, CancellationToken cancellationToken);
    Task AddFolderAsync(string did, string path, string folderName, CancellationToken cancellationToken);
    Task<Stream> DownloadFileAsync(string did, string filePath, CancellationToken? cancellationToken = null);
    Task MoveArtifactsAsync(string did, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task CopyArtifactsAsync(string did, IEnumerable<string> sourcesPath, string destinationPath, bool overwrite = false, CancellationToken? cancellationToken = null);
    Task RenameFileAsync(string did, string filePath, string newName, CancellationToken? cancellationToken = null);
    Task RenameFolderAsync(string did, string folderPath, string newName, CancellationToken? cancellationToken = null);
    Task DeleteArtifactsAsync(string did, IEnumerable<string> sourcesPath, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<string> GetFoldersAsync(string did, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);
    IAsyncEnumerable<string> GetFilesAsync(string did, string? path = null, string? searchText = null, CancellationToken? cancellationToken = null);
}
